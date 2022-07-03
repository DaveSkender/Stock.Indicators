using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class ParabolicSar : TestBase
{
    [TestMethod]
    public void Standard()
    {
        double acclerationStep = 0.02;
        double maxAccelerationFactor = 0.2;

        List<ParabolicSarResult> results =
            quotes.GetParabolicSar(acclerationStep, maxAccelerationFactor)
                .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Sar != null));

        // sample values
        ParabolicSarResult r14 = results[14];
        Assert.AreEqual(212.83, r14.Sar);
        Assert.AreEqual(true, r14.IsReversal);

        ParabolicSarResult r16 = results[16];
        Assert.AreEqual(212.9924, NullMath.Round(r16.Sar, 4));
        Assert.AreEqual(false, r16.IsReversal);

        ParabolicSarResult r94 = results[94];
        Assert.AreEqual(228.3600, NullMath.Round(r94.Sar, 4));
        Assert.AreEqual(false, r94.IsReversal);

        ParabolicSarResult r501 = results[501];
        Assert.AreEqual(229.7662, NullMath.Round(r501.Sar, 4));
        Assert.AreEqual(false, r501.IsReversal);
    }

    [TestMethod]
    public void Extended()
    {
        double acclerationStep = 0.02;
        double maxAccelerationFactor = 0.2;
        double initialStep = 0.01;

        List<ParabolicSarResult> results =
            quotes.GetParabolicSar(
                acclerationStep, maxAccelerationFactor, initialStep)
                .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Sar != null));

        // sample values
        ParabolicSarResult r14 = results[14];
        Assert.AreEqual(212.83, r14.Sar);
        Assert.AreEqual(true, r14.IsReversal);

        ParabolicSarResult r16 = results[16];
        Assert.AreEqual(212.9518, NullMath.Round(r16.Sar, 4));
        Assert.AreEqual(false, r16.IsReversal);

        ParabolicSarResult r94 = results[94];
        Assert.AreEqual(228.36, r94.Sar);
        Assert.AreEqual(false, r94.IsReversal);

        ParabolicSarResult r486 = results[486];
        Assert.AreEqual(273.4148, NullMath.Round(r486.Sar, 4));
        Assert.AreEqual(false, r486.IsReversal);

        ParabolicSarResult r501 = results[501];
        Assert.AreEqual(246.73, r501.Sar);
        Assert.AreEqual(false, r501.IsReversal);
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetParabolicSar()
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(479, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void InsufficientQuotes()
    {
        double acclerationStep = 0.02;
        double maxAccelerationFactor = 0.2;

        IEnumerable<Quote> insufficientQuotes = TestData.GetDefault()
            .OrderBy(x => x.Date)
            .Take(10);

        List<ParabolicSarResult> results =
            insufficientQuotes.GetParabolicSar(acclerationStep, maxAccelerationFactor)
                .ToList();

        // assertions

        // proper quantities
        Assert.AreEqual(10, results.Count);
        Assert.AreEqual(0, results.Count(x => x.Sar != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<ParabolicSarResult> r = Indicator.GetParabolicSar(badQuotes);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Sar is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<ParabolicSarResult> r0 = noquotes.GetParabolicSar();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<ParabolicSarResult> r1 = onequote.GetParabolicSar();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        double acclerationStep = 0.02;
        double maxAccelerationFactor = 0.2;

        List<ParabolicSarResult> results =
            quotes.GetParabolicSar(acclerationStep, maxAccelerationFactor)
                .RemoveWarmupPeriods()
                .ToList();

        // assertions
        Assert.AreEqual(488, results.Count);

        ParabolicSarResult last = results.LastOrDefault();
        Assert.AreEqual(229.7662, NullMath.Round(last.Sar, 4));
        Assert.AreEqual(false, last.IsReversal);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad acceleration step
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetParabolicSar(quotes, 0, 1));

        // insufficient acceleration step
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetParabolicSar(quotes, 0.02, 0));

        // step larger than factor
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetParabolicSar(quotes, 6, 2));

        // insufficient initial factor
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetParabolicSar(0.02, 0.5, 0));
    }
}
