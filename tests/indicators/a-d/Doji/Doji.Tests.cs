namespace Tests.Indicators.Series;

[TestClass]
public class DojiTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<CandleResult> results = Quotes
            .GetDoji()
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(112, results.Count(x => x.Match != Match.None));

        // sample values
        CandleResult r1 = results[1];
        Assert.AreEqual(null, r1.Price);
        Assert.AreEqual(0, (int)r1.Match);

        CandleResult r23 = results[23];
        Assert.AreEqual(216.28m, r23.Price);
        Assert.AreEqual(Match.Neutral, r23.Match);

        CandleResult r46 = results[46];
        Assert.AreEqual(null, r46.Price);
        Assert.AreEqual(Match.None, r46.Match);

        CandleResult r392 = results[392];
        Assert.AreEqual(null, r392.Price);
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
        List<CandleResult> r = BadQuotes
            .GetDoji()
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<CandleResult> r0 = Noquotes
            .GetDoji()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<CandleResult> r1 = Onequote
            .GetDoji()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        List<CandleResult> r = Quotes
            .GetDoji()
            .Condense()
            .ToList();

        Assert.AreEqual(112, r.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad maximum change value
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetDoji(-0.00001));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetDoji(0.50001));
    }
}
