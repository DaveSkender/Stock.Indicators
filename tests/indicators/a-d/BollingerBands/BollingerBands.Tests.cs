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

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Sma != null).Count());
        Assert.AreEqual(483, results.Where(x => x.UpperBand != null).Count());
        Assert.AreEqual(483, results.Where(x => x.LowerBand != null).Count());
        Assert.AreEqual(483, results.Where(x => x.PercentB != null).Count());
        Assert.AreEqual(483, results.Where(x => x.ZScore != null).Count());
        Assert.AreEqual(483, results.Where(x => x.Width != null).Count());

        // sample values
        BollingerBandsResult r1 = results[249];
        Assert.AreEqual(255.5500m, Math.Round((decimal)r1.Sma, 4));
        Assert.AreEqual(259.5642m, Math.Round((decimal)r1.UpperBand, 4));
        Assert.AreEqual(251.5358m, Math.Round((decimal)r1.LowerBand, 4));
        Assert.AreEqual(0.803923, Math.Round((double)r1.PercentB, 6));
        Assert.AreEqual(1.215692, Math.Round((double)r1.ZScore, 6));
        Assert.AreEqual(0.031416, Math.Round((double)r1.Width, 6));

        BollingerBandsResult r2 = results[501];
        Assert.AreEqual(251.8600m, Math.Round((decimal)r2.Sma, 4));
        Assert.AreEqual(273.7004m, Math.Round((decimal)r2.UpperBand, 4));
        Assert.AreEqual(230.0196m, Math.Round((decimal)r2.LowerBand, 4));
        Assert.AreEqual(0.349362, Math.Round((double)r2.PercentB, 6));
        Assert.AreEqual(-0.602552, Math.Round((double)r2.ZScore, 6));
        Assert.AreEqual(0.173433, Math.Round((double)r2.Width, 6));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<BollingerBandsResult> r = Indicator.GetBollingerBands(badQuotes, 15, 3);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<BollingerBandsResult> r0 = noquotes.GetBollingerBands();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<BollingerBandsResult> r1 = onequote.GetBollingerBands();
        Assert.AreEqual(1, r1.Count());
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
        Assert.AreEqual(251.8600m, Math.Round((decimal)last.Sma, 4));
        Assert.AreEqual(273.7004m, Math.Round((decimal)last.UpperBand, 4));
        Assert.AreEqual(230.0196m, Math.Round((decimal)last.LowerBand, 4));
        Assert.AreEqual(0.349362, Math.Round((double)last.PercentB, 6));
        Assert.AreEqual(-0.602552, Math.Round((double)last.ZScore, 6));
        Assert.AreEqual(0.173433, Math.Round((double)last.Width, 6));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetBollingerBands(quotes, 1));

        // bad standard deviation
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetBollingerBands(quotes, 2, 0));
    }
}
