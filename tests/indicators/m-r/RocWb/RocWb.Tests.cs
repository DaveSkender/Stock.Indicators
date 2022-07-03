using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class RocWb : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<RocWbResult> results = quotes.GetRocWb(20, 3, 20).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Roc != null));
        Assert.AreEqual(480, results.Count(x => x.RocEma != null));
        Assert.AreEqual(463, results.Count(x => x.UpperBand != null));
        Assert.AreEqual(463, results.Count(x => x.LowerBand != null));

        // sample values
        RocWbResult r19 = results[19];
        Assert.IsNull(r19.Roc);
        Assert.IsNull(r19.RocEma);
        Assert.IsNull(r19.UpperBand);
        Assert.IsNull(r19.LowerBand);

        RocWbResult r20 = results[20];
        Assert.AreEqual(1.0573, Math.Round(r20.Roc.Value, 4));
        Assert.IsNull(r20.RocEma);
        Assert.IsNull(r20.UpperBand);
        Assert.IsNull(r20.LowerBand);

        RocWbResult r22 = results[22];
        Assert.AreEqual(0.9617, Math.Round(r22.RocEma.Value, 4));
        Assert.IsNull(r22.UpperBand);
        Assert.IsNull(r22.LowerBand);

        RocWbResult r23 = results[23];
        Assert.AreEqual(0.8582, Math.Round(r23.RocEma.Value, 4));
        Assert.IsNull(r23.UpperBand);
        Assert.IsNull(r23.LowerBand);

        RocWbResult r38 = results[38];
        Assert.AreEqual(3.6872, Math.Round(r38.RocEma.Value, 4));
        Assert.IsNull(r38.UpperBand);
        Assert.IsNull(r38.LowerBand);

        RocWbResult r39 = results[39];
        Assert.AreEqual(4.5348, Math.Round(r39.RocEma.Value, 4));
        Assert.AreEqual(3.0359, Math.Round(r39.UpperBand.Value, 4));
        Assert.AreEqual(-3.0359, Math.Round(r39.LowerBand.Value, 4));

        RocWbResult r49 = results[49];
        Assert.AreEqual(2.3147, Math.Round(r49.RocEma.Value, 4));
        Assert.AreEqual(3.6761, Math.Round(r49.UpperBand.Value, 4));

        RocWbResult r149 = results[149];
        Assert.AreEqual(1.7377, Math.Round(r149.UpperBand.Value, 4));

        RocWbResult r249 = results[249];
        Assert.AreEqual(3.0683, Math.Round(r249.UpperBand.Value, 4));

        RocWbResult r501 = results[501];
        Assert.AreEqual(-8.2482, Math.Round(r501.Roc.Value, 4));
        Assert.AreEqual(-8.3390, Math.Round(r501.RocEma.Value, 4));
        Assert.AreEqual(6.1294, Math.Round(r501.UpperBand.Value, 4));
        Assert.AreEqual(-6.1294, Math.Round(r501.LowerBand.Value, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<RocWbResult> results = quotes
            .Use(CandlePart.Close)
            .GetRocWb(20, 3, 20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(482, results.Count(x => x.Roc != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<RocWbResult> r = tupleNanny.GetRocWb(6, 7, 5);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.UpperBand is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<RocWbResult> results = quotes
            .GetSma(2)
            .GetRocWb(20, 3, 20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(481, results.Count(x => x.Roc != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetRocWb(20, 3, 20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(473, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<RocWbResult> r = Indicator.GetRocWb(badQuotes, 35, 3, 35);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Roc is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<RocWbResult> r0 = noquotes.GetRocWb(5, 3, 2);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<RocWbResult> r1 = onequote.GetRocWb(5, 3, 2);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<RocWbResult> results = quotes.GetRocWb(20, 3, 20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (20 + 3 + 100), results.Count);

        RocWbResult last = results.LastOrDefault();
        Assert.AreEqual(-8.2482, Math.Round(last.Roc.Value, 4));
        Assert.AreEqual(-8.3390, Math.Round(last.RocEma.Value, 4));
        Assert.AreEqual(6.1294, Math.Round(last.UpperBand.Value, 4));
        Assert.AreEqual(-6.1294, Math.Round(last.LowerBand.Value, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetRocWb(quotes, 0, 3, 12));

        // bad EMA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetRocWb(quotes, 14, 0, 14));

        // bad STDDEV period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetRocWb(quotes, 15, 3, 16));
    }
}
