namespace Tests.Indicators;

[TestClass]
public class MarubozuTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<CandleResult> results = quotes
            .GetMarubozu(95)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(6, results.Count(x => x.Match != Match.None));

        // sample values
        CandleResult r31 = results[31];
        Assert.IsNull(r31.Price);
        Assert.AreEqual(0, (int)r31.Match);

        CandleResult r32 = results[32];
        Assert.AreEqual(222.10m, r32.Price);
        Assert.AreEqual(Match.BullSignal, r32.Match);

        CandleResult r33 = results[33];
        Assert.IsNull(r33.Price);
        Assert.AreEqual(Match.None, r33.Match);

        CandleResult r34 = results[34];
        Assert.IsNull(r34.Price);
        Assert.AreEqual(Match.None, r34.Match);

        CandleResult r274 = results[274];
        Assert.IsNull(r274.Price);
        Assert.AreEqual(Match.None, r274.Match);

        CandleResult r277 = results[277];
        Assert.AreEqual(248.13m, r277.Price);
        Assert.AreEqual(Match.BearSignal, r277.Match);
    }

    [TestMethod]
    public void BadData()
    {
        List<CandleResult> r = badQuotes
            .GetMarubozu()
            .ToList();

        Assert.HasCount(502, r);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<CandleResult> r0 = noquotes
            .GetMarubozu()
            .ToList();

        Assert.IsEmpty(r0);

        List<CandleResult> r1 = onequote
            .GetMarubozu()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Condense()
    {
        List<CandleResult> r = quotes
            .GetMarubozu(95)
            .Condense()
            .ToList();

        Assert.HasCount(6, r);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad minimum body percent values
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetMarubozu(79.9));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetMarubozu(100.1));
    }
}
