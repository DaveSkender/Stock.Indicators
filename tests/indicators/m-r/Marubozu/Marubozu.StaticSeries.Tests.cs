namespace StaticSeries;

[TestClass]
public class Marubozu : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<CandleResult> results = Quotes
            .ToMarubozu();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(6, results.Where(static x => x.Match != Match.None));

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
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<CandleResult> r = BadQuotes
            .ToMarubozu();

        Assert.HasCount(502, r);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<CandleResult> r0 = Noquotes
            .ToMarubozu();

        Assert.IsEmpty(r0);

        IReadOnlyList<CandleResult> r1 = Onequote
            .ToMarubozu();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<CandleResult> results = Quotes
            .ToMarubozu()
            .Condense();

        Assert.HasCount(6, results);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad minimum body percent values
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToMarubozu(79.9));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToMarubozu(100.1));
    }
}
