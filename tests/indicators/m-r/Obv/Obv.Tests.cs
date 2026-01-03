namespace Tests.Indicators;

[TestClass]
public class ObvTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<ObvResult> results = quotes
            .GetObv()
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(502, results.Where(static x => x.ObvSma == null));

        // sample values
        ObvResult r1 = results[249];
        Assert.AreEqual(1780918888, r1.Obv);
        Assert.IsNull(r1.ObvSma);

        ObvResult r2 = results[501];
        Assert.AreEqual(539843504, r2.Obv);
        Assert.IsNull(r2.ObvSma);
    }

    [TestMethod]
    public void WithSma()
    {
        List<ObvResult> results = quotes
            .GetObv(20)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.ObvSma != null));

        // sample values
        ObvResult r1 = results[501];
        Assert.AreEqual(539843504, r1.Obv);
        Assert.AreEqual(1016208844.40, r1.ObvSma);
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetObv()
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<ObvResult> r = badQuotes
            .GetObv()
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => double.IsNaN(x.Obv)));
    }

    [TestMethod]
    public void BigData()
    {
        List<ObvResult> r = bigQuotes
            .GetObv()
            .ToList();

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<ObvResult> r0 = noquotes
            .GetObv()
            .ToList();

        Assert.IsEmpty(r0);

        List<ObvResult> r1 = onequote
            .GetObv()
            .ToList();

        Assert.HasCount(1, r1);
    }

    // bad SMA period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetObv(0));
}
