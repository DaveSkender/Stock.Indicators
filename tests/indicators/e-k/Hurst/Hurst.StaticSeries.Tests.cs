namespace StaticSeries;

[TestClass]
public class Hurst : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<HurstResult> results = LongestQuotes
            .ToHurst(LongestQuotes.Count - 1);

        // assertions

        // proper quantities
        Assert.HasCount(15821, results);
        Assert.HasCount(1, results.Where(static x => x.HurstExponent != null));

        // sample value
        HurstResult r15820 = results[15820];
        Assert.AreEqual(0.483563, r15820.HurstExponent.Round(6));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<HurstResult> results = Quotes
            .Use(CandlePart.Close)
            .ToHurst();

        Assert.HasCount(502, results);
        Assert.AreEqual(402, results.Count(static x => x.HurstExponent != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToHurst()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.AreEqual(393, results.Count(static x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<HurstResult> results = Quotes
            .ToSma(10)
            .ToHurst();

        Assert.HasCount(502, results);
        Assert.AreEqual(393, results.Count(static x => x.HurstExponent != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<HurstResult> r = BadQuotes
            .ToHurst(150);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.HurstExponent is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<HurstResult> r0 = Noquotes
            .ToHurst();

        Assert.IsEmpty(r0);

        IReadOnlyList<HurstResult> r1 = Onequote
            .ToHurst();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<HurstResult> results = LongestQuotes.ToHurst(LongestQuotes.Count - 1)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(1, results);

        HurstResult last = results[^1];
        Assert.AreEqual(0.483563, last.HurstExponent.Round(6));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToHurst(19));
}
