namespace Tests.Indicators;

[TestClass]
public class HurstTests : TestBase
{
    [TestMethod]
    public void StandardLong()
    {
        List<HurstResult> results = longestQuotes
            .GetHurst(longestQuotes.Count() - 1)
            .ToList();

        // assertions

        // proper quantities
        Assert.HasCount(15821, results);
        Assert.AreEqual(1, results.Count(static x => x.HurstExponent != null));

        // sample value
        HurstResult r15820 = results[15820];
        Assert.AreEqual(0.483563, r15820.HurstExponent.Round(6));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<HurstResult> results = quotes
            .Use(CandlePart.Close)
            .GetHurst(100)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(402, results.Count(static x => x.HurstExponent != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<HurstResult> r = tupleNanny
            .GetHurst(100)
            .ToList();

        Assert.HasCount(200, r);
        Assert.IsEmpty(r.Where(static x => x.HurstExponent is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetHurst(100)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(393, results.Count(static x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<HurstResult> results = quotes
            .GetSma(10)
            .GetHurst(100)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(393, results.Count(static x => x.HurstExponent != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<HurstResult> r = badQuotes
            .GetHurst(150)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.HurstExponent is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<HurstResult> r0 = noquotes
            .GetHurst()
            .ToList();

        Assert.IsEmpty(r0);

        List<HurstResult> r1 = onequote
            .GetHurst()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<HurstResult> results = longestQuotes.GetHurst(longestQuotes.Count() - 1)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(1, results);

        HurstResult last = results.LastOrDefault();
        Assert.AreEqual(0.483563, last.HurstExponent.Round(6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetHurst(19));
}
