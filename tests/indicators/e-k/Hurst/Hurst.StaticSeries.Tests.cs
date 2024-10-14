namespace StaticSeries;

[TestClass]
public class Hurst : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<HurstResult> results = LongestQuotes
            .GetHurst(LongestQuotes.Count - 1);

        // assertions

        // proper quantities
        Assert.AreEqual(15821, results.Count);
        Assert.AreEqual(1, results.Count(x => x.HurstExponent != null));

        // sample value
        HurstResult r15820 = results[15820];
        Assert.AreEqual(0.483563, r15820.HurstExponent.Round(6));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<HurstResult> results = Quotes
            .Use(CandlePart.Close)
            .GetHurst();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(402, results.Count(x => x.HurstExponent != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetHurst()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(393, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<HurstResult> results = Quotes
            .ToSma(10)
            .GetHurst();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(393, results.Count(x => x.HurstExponent != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<HurstResult> r = BadQuotes
            .GetHurst(150);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.HurstExponent is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<HurstResult> r0 = Noquotes
            .GetHurst();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<HurstResult> r1 = Onequote
            .GetHurst();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<HurstResult> results = LongestQuotes.GetHurst(LongestQuotes.Count - 1)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(1, results.Count);

        HurstResult last = results[^1];
        Assert.AreEqual(0.483563, last.HurstExponent.Round(6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetHurst(19));
}
