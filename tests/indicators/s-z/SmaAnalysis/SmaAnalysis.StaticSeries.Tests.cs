namespace StaticSeries;

[TestClass]
public class SmaAnalyses : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<SmaAnalysisResult> results = Quotes
            .ToSmaAnalysis(20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Sma != null));

        // sample value
        SmaAnalysisResult r = results[501];
        Assert.AreEqual(251.86, r.Sma.Round(6));
        Assert.AreEqual(9.450000, r.Mad.Round(6));
        Assert.AreEqual(119.25102, r.Mse.Round(6));
        Assert.AreEqual(0.037637, r.Mape.Round(6));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<SmaAnalysisResult> results = Quotes
            .Use(CandlePart.Close)
            .ToSmaAnalysis(20);

        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<SmaAnalysisResult> results = Quotes
            .ToSma(2)
            .ToSmaAnalysis(20);

        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .ToSmaAnalysis(10)
            .ToEma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(484, results.Where(static x => x.Ema != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<SmaAnalysisResult> r = BadQuotes
            .ToSmaAnalysis(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Mape is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<SmaAnalysisResult> r0 = Noquotes
            .ToSmaAnalysis(6);

        Assert.IsEmpty(r0);

        IReadOnlyList<SmaAnalysisResult> r1 = Onequote
            .ToSmaAnalysis(6);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<SmaAnalysisResult> results = Quotes
            .ToSmaAnalysis(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 19, results);
        Assert.AreEqual(251.8600, Math.Round(results[^1].Sma.Value, 4));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToSmaAnalysis(0));
}
