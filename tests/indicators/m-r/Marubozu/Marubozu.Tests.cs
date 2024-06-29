namespace Tests.Indicators.Series;

[TestClass]
public class MarubozuTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<CandleResult> results = Quotes
            .GetMarubozu()
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(6, results.Count(x => x.Match != Match.None));

        // sample values
        CandleResult r31 = results[31];
        Assert.AreEqual(null, r31.Price);
        Assert.AreEqual(0, (int)r31.Match);

        CandleResult r32 = results[32];
        Assert.AreEqual(222.10m, r32.Price);
        Assert.AreEqual(Match.BullSignal, r32.Match);

        CandleResult r33 = results[33];
        Assert.AreEqual(null, r33.Price);
        Assert.AreEqual(Match.None, r33.Match);

        CandleResult r34 = results[34];
        Assert.AreEqual(null, r34.Price);
        Assert.AreEqual(Match.None, r34.Match);

        CandleResult r274 = results[274];
        Assert.AreEqual(null, r274.Price);
        Assert.AreEqual(Match.None, r274.Match);

        CandleResult r277 = results[277];
        Assert.AreEqual(248.13m, r277.Price);
        Assert.AreEqual(Match.BearSignal, r277.Match);
    }

    [TestMethod]
    public override void BadData()
    {
        List<CandleResult> r = BadQuotes
            .GetMarubozu()
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<CandleResult> r0 = Noquotes
            .GetMarubozu()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<CandleResult> r1 = Onequote
            .GetMarubozu()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        List<CandleResult> r = Quotes
            .GetMarubozu()
            .Condense()
            .ToList();

        Assert.AreEqual(6, r.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad minimum body percent values
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetMarubozu(79.9));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetMarubozu(100.1));
    }
}
