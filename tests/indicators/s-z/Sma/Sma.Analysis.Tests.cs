namespace Tests.Indicators;

[TestClass]
public class SmaExtendedTests : TestBase
{
    [TestMethod]
    public void Analysis()
    {
        List<SmaAnalysis> results = quotes
            .GetSmaAnalysis(20)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample value
        SmaAnalysis r = results[501];
        Assert.AreEqual(251.86, r.Sma.Round(6));
        Assert.AreEqual(9.450000, r.Mad.Round(6));
        Assert.AreEqual(119.25102, r.Mse.Round(6));
        Assert.AreEqual(0.037637, r.Mape.Round(6));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<SmaAnalysis> results = quotes
            .Use(CandlePart.Close)
            .GetSmaAnalysis(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<SmaAnalysis> r = tupleNanny
            .GetSmaAnalysis(6)
            .ToList();

        Assert.HasCount(200, r);
        Assert.IsEmpty(r.Where(x => x.Mse is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        List<SmaAnalysis> results = quotes
            .GetSma(2)
            .GetSmaAnalysis(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(482, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<EmaResult> results = quotes
            .GetSmaAnalysis(10)
            .GetEma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(484, results.Count(x => x.Ema != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<SmaAnalysis> r = badQuotes
            .GetSmaAnalysis(15)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Mape is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<SmaAnalysis> r0 = noquotes
            .GetSmaAnalysis(6)
            .ToList();

        Assert.IsEmpty(r0);

        List<SmaAnalysis> r1 = onequote
            .GetSmaAnalysis(6)
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<SmaAnalysis> results = quotes
            .GetSmaAnalysis(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 19, results);
        Assert.AreEqual(251.8600, Math.Round(results.LastOrDefault().Sma.Value, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetSmaAnalysis(0));
}
