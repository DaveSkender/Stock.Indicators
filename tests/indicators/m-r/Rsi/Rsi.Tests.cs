namespace Tests.Indicators;

[TestClass]
public class RsiTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<RsiResult> results = quotes
            .GetRsi(14)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(static x => x.Rsi != null));

        // sample values
        RsiResult r1 = results[13];
        Assert.IsNull(r1.Rsi);

        RsiResult r2 = results[14];
        Assert.AreEqual(62.0541, r2.Rsi.Round(4));

        RsiResult r3 = results[249];
        Assert.AreEqual(70.9368, r3.Rsi.Round(4));

        RsiResult r4 = results[501];
        Assert.AreEqual(42.0773, r4.Rsi.Round(4));
    }

    [TestMethod]
    public void SmallLookback()
    {
        int lookbackPeriods = 1;
        List<RsiResult> results = quotes
            .GetRsi(lookbackPeriods)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(501, results.Where(static x => x.Rsi != null));

        // sample values
        RsiResult r1 = results[28];
        Assert.AreEqual(100, r1.Rsi);

        RsiResult r2 = results[52];
        Assert.AreEqual(0, r2.Rsi);
    }

    [TestMethod]
    public void CryptoData()
    {
        IEnumerable<Quote> btc = TestData.GetBitcoin();

        List<RsiResult> r = btc
            .GetRsi(1)
            .ToList();

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public void UseTuple()
    {
        List<RsiResult> results = quotes
            .Use(CandlePart.Close)
            .GetRsi(14)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(static x => x.Rsi != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<RsiResult> r = tupleNanny
            .GetRsi(6)
            .ToList();

        Assert.HasCount(200, r);
        Assert.IsEmpty(r.Where(static x => x.Rsi is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        List<RsiResult> results = quotes
            .GetSma(2)
            .GetRsi(14)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(487, results.Where(static x => x.Rsi != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetRsi(14)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(479, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public void NaN()
    {
        IEnumerable<RsiResult> r = TestData.GetBtcUsdNan()
            .GetRsi(14);

        Assert.IsEmpty(r.Where(static x => x.Rsi is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void BadData()
    {
        List<RsiResult> r = badQuotes
            .GetRsi(20)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Rsi is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<RsiResult> r0 = noquotes
            .GetRsi()
            .ToList();

        Assert.IsEmpty(r0);

        List<RsiResult> r1 = onequote
            .GetRsi()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<RsiResult> results = quotes
            .GetRsi(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - (10 * 14), results);

        RsiResult last = results.LastOrDefault();
        Assert.AreEqual(42.0773, last.Rsi.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetRsi(0));
}
