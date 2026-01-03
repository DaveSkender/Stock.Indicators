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
        Assert.HasCount(502, results);
        Assert.AreEqual(112, results.Count(static x => x.Match != Match.None));

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
    public void BadData()
    {
        List<CandleResult> r = badQuotes
            .GetDoji()
            .ToList();

        Assert.HasCount(502, r);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<CandleResult> r0 = noquotes
            .GetDoji()
            .ToList();

        Assert.IsEmpty(r0);

        List<CandleResult> r1 = onequote
            .GetDoji()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Condense()
    {
        List<CandleResult> r = quotes
            .GetDoji(0.1)
            .Condense()
            .ToList();

        Assert.HasCount(112, r);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad maximum change value
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetDoji(-0.00001));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetDoji(0.50001));
    }
}
