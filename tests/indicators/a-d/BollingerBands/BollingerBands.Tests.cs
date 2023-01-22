using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class BollingerBandsTests : TestBase
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
        Assert.AreEqual(255.5500, r1.Sma.Round(4));
        Assert.AreEqual(259.5642, r1.UpperBand.Round(4));
        Assert.AreEqual(251.5358, r1.LowerBand.Round(4));
        Assert.AreEqual(0.803923, r1.PercentB.Round(6));
        Assert.AreEqual(1.215692, r1.ZScore.Round(6));
        Assert.AreEqual(0.031416, r1.Width.Round(6));

        BollingerBandsResult r2 = results[501];
        Assert.AreEqual(251.8600, r2.Sma.Round(4));
        Assert.AreEqual(273.7004, r2.UpperBand.Round(4));
        Assert.AreEqual(230.0196, r2.LowerBand.Round(4));
        Assert.AreEqual(0.349362, r2.PercentB.Round(6));
        Assert.AreEqual(-0.602552, r2.ZScore.Round(6));
        Assert.AreEqual(0.173433, r2.Width.Round(6));
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
        Assert.AreEqual(251.8600, last.Sma.Round(4));
        Assert.AreEqual(273.7004, last.UpperBand.Round(4));
        Assert.AreEqual(230.0196, last.LowerBand.Round(4));
        Assert.AreEqual(0.349362, last.PercentB.Round(6));
        Assert.AreEqual(-0.602552, last.ZScore.Round(6));
        Assert.AreEqual(0.173433, last.Width.Round(6));
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
