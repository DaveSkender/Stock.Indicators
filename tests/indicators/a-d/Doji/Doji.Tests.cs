namespace Tests.Indicators;

[TestClass]
public class DojiTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<CandleResult> results = quotes
            .GetDoji(0.1)
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
    public void BadData()
    {
        List<CandleResult> r = badQuotes
            .GetDoji()
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<CandleResult> r0 = noquotes
            .GetDoji()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<CandleResult> r1 = onequote
            .GetDoji()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        List<CandleResult> r = quotes
            .GetDoji(0.1)
            .Condense()
            .ToList();

        Assert.AreEqual(112, r.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad maximum change value
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetDoji(-0.00001));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetDoji(0.50001));
    }
}
