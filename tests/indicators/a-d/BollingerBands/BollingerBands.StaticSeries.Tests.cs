namespace StaticSeries;

[TestClass]
public class BollingerBands : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<BollingerBandsResult> results =
            Quotes.ToBollingerBands();

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
    public void UseReusable()
    {
        IReadOnlyList<BollingerBandsResult> results = Quotes
            .Use(CandlePart.Close)
            .ToBollingerBands();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<BollingerBandsResult> results = Quotes
            .ToSma(2)
            .ToBollingerBands();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.UpperBand != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToBollingerBands()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<BollingerBandsResult> r = BadQuotes
            .ToBollingerBands(15, 3);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UpperBand is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<BollingerBandsResult> r0 = Noquotes
            .ToBollingerBands();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<BollingerBandsResult> r1 = Onequote
            .ToBollingerBands();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<BollingerBandsResult> results = Quotes
            .ToBollingerBands()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        BollingerBandsResult last = results[^1];
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
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToBollingerBands(1));

        // bad standard deviation
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToBollingerBands(2, 0));
    }
}
