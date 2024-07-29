namespace StaticSeries;

[TestClass]
public class SmaAnalyses : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<SmaAnalysis> results = Quotes
            .GetSmaAnalysis(20);

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
        IReadOnlyList<SmaAnalysis> results = Quotes
            .Use(CandlePart.Close)
            .GetSmaAnalysis(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<SmaAnalysis> results = Quotes
            .GetSma(2)
            .GetSmaAnalysis(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .GetSmaAnalysis(10)
            .GetEma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(484, results.Count(x => x.Ema != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<SmaAnalysis> r = BadQuotes
            .GetSmaAnalysis(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Mape is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<SmaAnalysis> r0 = Noquotes
            .GetSmaAnalysis(6);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<SmaAnalysis> r1 = Onequote
            .GetSmaAnalysis(6);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<SmaAnalysis> results = Quotes
            .GetSmaAnalysis(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);
        Assert.AreEqual(251.8600, Math.Round(results[^1].Sma.Value, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetSmaAnalysis(0));
}
