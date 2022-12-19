using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class BollingerBands : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<BollingerBandsResult> results =
            quotes.GetBollingerBands(20, 2)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));
        Assert.AreEqual(483, results.Count(x => x.UpperBand != null));
        Assert.AreEqual(483, results.Count(x => x.LowerBand != null));
        Assert.AreEqual(483, results.Count(x => x.PercentB != null));
        Assert.AreEqual(483, results.Count(x => x.ZScore != null));
        Assert.AreEqual(483, results.Count(x => x.Width != null));

        // sample values
        BollingerBandsResult r1 = results[249];
        Assert.AreEqual(255.5500, NullMath.Round(r1.Sma, 4));
        Assert.AreEqual(259.5642, NullMath.Round(r1.UpperBand, 4));
        Assert.AreEqual(251.5358, NullMath.Round(r1.LowerBand, 4));
        Assert.AreEqual(0.803923, NullMath.Round(r1.PercentB, 6));
        Assert.AreEqual(1.215692, NullMath.Round(r1.ZScore, 6));
        Assert.AreEqual(0.031416, NullMath.Round(r1.Width, 6));

        BollingerBandsResult r2 = results[501];
        Assert.AreEqual(251.8600, NullMath.Round(r2.Sma, 4));
        Assert.AreEqual(273.7004, NullMath.Round(r2.UpperBand, 4));
        Assert.AreEqual(230.0196, NullMath.Round(r2.LowerBand, 4));
        Assert.AreEqual(0.349362, NullMath.Round(r2.PercentB, 6));
        Assert.AreEqual(-0.602552, NullMath.Round(r2.ZScore, 6));
        Assert.AreEqual(0.173433, NullMath.Round(r2.Width, 6));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<BollingerBandsResult> results = quotes
            .Use(CandlePart.Close)
            .GetBollingerBands()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<BollingerBandsResult> r = tupleNanny
            .GetBollingerBands()
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UpperBand is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<BollingerBandsResult> results = quotes
            .GetSma(2)
            .GetBollingerBands()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.UpperBand != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetBollingerBands()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<BollingerBandsResult> r = badQuotes
            .GetBollingerBands(15, 3)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UpperBand is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<BollingerBandsResult> r0 = noquotes
            .GetBollingerBands()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<BollingerBandsResult> r1 = onequote
            .GetBollingerBands()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<BollingerBandsResult> results =
            quotes.GetBollingerBands(20, 2)
                .RemoveWarmupPeriods()
                .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        BollingerBandsResult last = results.LastOrDefault();
        Assert.AreEqual(251.8600, NullMath.Round(last.Sma, 4));
        Assert.AreEqual(273.7004, NullMath.Round(last.UpperBand, 4));
        Assert.AreEqual(230.0196, NullMath.Round(last.LowerBand, 4));
        Assert.AreEqual(0.349362, NullMath.Round(last.PercentB, 6));
        Assert.AreEqual(-0.602552, NullMath.Round(last.ZScore, 6));
        Assert.AreEqual(0.173433, NullMath.Round(last.Width, 6));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetBollingerBands(1));

        // bad standard deviation
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetBollingerBands(2, 0));
    }
}
