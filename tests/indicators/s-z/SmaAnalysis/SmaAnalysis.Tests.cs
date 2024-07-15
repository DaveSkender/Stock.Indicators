namespace Tests.Indicators.Series;

[TestClass]
public class SmaAnalysisTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<SmaAnalysis> results = Quotes
            .GetSmaAnalysis(20)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample value
        SmaAnalysis r = results[501];
        Assert.AreEqual(251.86, r.Sma.Round(6));
        Assert.AreEqual(9.450000, r.Mad.Round(6));
        Assert.AreEqual(119.25102, r.Mse.Round(6));
        Assert.AreEqual(0.037637, r.Mape.Round(6));
    }

    [TestMethod]
    public void UseReusable()
    {
        List<SmaAnalysis> results = Quotes
            .Use(CandlePart.Close)
            .GetSmaAnalysis(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<SmaAnalysis> results = Quotes
            .GetSma(2)
            .GetSmaAnalysis(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<EmaResult> results = Quotes
            .GetSmaAnalysis(10)
            .GetEma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(484, results.Count(x => x.Ema != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<SmaAnalysis> r = BadQuotes
            .GetSmaAnalysis(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Mape is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<SmaAnalysis> r0 = Noquotes
            .GetSmaAnalysis(6)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<SmaAnalysis> r1 = Onequote
            .GetSmaAnalysis(6)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<SmaAnalysis> results = Quotes
            .GetSmaAnalysis(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);
        Assert.AreEqual(251.8600, Math.Round(results.LastOrDefault().Sma.Value, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetSmaAnalysis(0));
}
