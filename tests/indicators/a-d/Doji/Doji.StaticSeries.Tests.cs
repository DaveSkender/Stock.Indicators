namespace StaticSeries;

[TestClass]
public class Doji : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<CandleResult> results = Quotes
            .ToDoji();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(112, results.Count(x => x.Match != Match.None));

        // sample values
        CandleResult r1 = results[1];
        Assert.IsNull(r1.Price);
        Assert.AreEqual(0, (int)r1.Match);

        CandleResult r23 = results[23];
        Assert.AreEqual(216.28m, r23.Price);
        Assert.AreEqual(Match.Neutral, r23.Match);

        CandleResult r46 = results[46];
        Assert.IsNull(r46.Price);
        Assert.AreEqual(Match.None, r46.Match);

        CandleResult r392 = results[392];
        Assert.IsNull(r392.Price);
        Assert.AreEqual(Match.None, r392.Match);

        CandleResult r451 = results[451];
        Assert.AreEqual(273.64m, r451.Price);
        Assert.AreEqual(1, (int)r451.Match);

        CandleResult r477 = results[477];
        Assert.AreEqual(256.86m, r477.Price);
        Assert.AreEqual(Match.Neutral, r477.Match);
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<CandleResult> r = BadQuotes
            .ToDoji();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<CandleResult> r0 = Noquotes
            .ToDoji();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<CandleResult> r1 = Onequote
            .ToDoji();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<CandleResult> results = Quotes
            .ToDoji()
            .Condense();

        Assert.AreEqual(112, results.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad maximum change value
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToDoji(-0.00001));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToDoji(0.50001));
    }
}
