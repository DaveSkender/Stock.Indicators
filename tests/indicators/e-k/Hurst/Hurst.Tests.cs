namespace Series;

[TestClass]
public class HurstTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<HurstResult> results = LongestQuotes
            .GetHurst(LongestQuotes.Count - 1)
            .ToList();

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
        List<HurstResult> results = Quotes
            .Use(CandlePart.Close)
            .GetHurst()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(402, results.Count(x => x.HurstExponent != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetHurst()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(393, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<HurstResult> results = Quotes
            .GetSma(10)
            .GetHurst()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(393, results.Count(x => x.HurstExponent != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<HurstResult> r = BadQuotes
            .GetHurst(150)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.HurstExponent is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<HurstResult> r0 = Noquotes
            .GetHurst()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<HurstResult> r1 = Onequote
            .GetHurst()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<HurstResult> results = LongestQuotes.GetHurst(LongestQuotes.Count - 1)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(1, results.Count);

        HurstResult last = results.LastOrDefault();
        Assert.AreEqual(0.483563, last.HurstExponent.Round(6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetHurst(19));
}
