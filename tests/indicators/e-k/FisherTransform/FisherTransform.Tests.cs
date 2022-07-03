using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class FisherTransform : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<FisherTransformResult> results = quotes.GetFisherTransform(10)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.Fisher != 0));

        // sample values
        Assert.AreEqual(0, results[0].Fisher);
        Assert.IsNull(results[0].Trigger);

        Assert.AreEqual(0.3428, Math.Round(results[1].Fisher.Value, 4));
        Assert.AreEqual(0, results[1].Trigger);

        Assert.AreEqual(0.6873, Math.Round(results[2].Fisher.Value, 4));
        Assert.AreEqual(0.3428, Math.Round(results[2].Trigger.Value, 4));

        Assert.AreEqual(1.3324, Math.Round(results[9].Fisher.Value, 4));
        Assert.AreEqual(1.4704, Math.Round(results[9].Trigger.Value, 4));

        Assert.AreEqual(0.9790, Math.Round(results[10].Fisher.Value, 4));
        Assert.AreEqual(1.3324, Math.Round(results[10].Trigger.Value, 4));

        Assert.AreEqual(6.1509, Math.Round(results[35].Fisher.Value, 4));
        Assert.AreEqual(4.7014, Math.Round(results[35].Trigger.Value, 4));

        Assert.AreEqual(5.4455, Math.Round(results[36].Fisher.Value, 4));
        Assert.AreEqual(6.1509, Math.Round(results[36].Trigger.Value, 4));

        Assert.AreEqual(1.0349, Math.Round(results[149].Fisher.Value, 4));
        Assert.AreEqual(0.7351, Math.Round(results[149].Trigger.Value, 4));

        Assert.AreEqual(1.3496, Math.Round(results[249].Fisher.Value, 4));
        Assert.AreEqual(1.4408, Math.Round(results[249].Trigger.Value, 4));

        Assert.AreEqual(-1.2876, Math.Round(results[501].Fisher.Value, 4));
        Assert.AreEqual(-2.0071, Math.Round(results[501].Trigger.Value, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<FisherTransformResult> results = quotes
            .Use(CandlePart.Close)
            .GetFisherTransform(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(501, results.Count(x => x.Fisher != 0));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<FisherTransformResult> r = tupleNanny.GetFisherTransform(6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Fisher is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<FisherTransformResult> results = quotes
            .GetSma(2)
            .GetFisherTransform(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(501, results.Count(x => x.Fisher != 0));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetFisherTransform(10)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<FisherTransformResult> r = Indicator.GetFisherTransform(badQuotes, 9);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Fisher is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<FisherTransformResult> r0 = noquotes.GetFisherTransform();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<FisherTransformResult> r1 = onequote.GetFisherTransform();
        Assert.AreEqual(1, r1.Count());
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetFisherTransform(quotes, 0));
}
