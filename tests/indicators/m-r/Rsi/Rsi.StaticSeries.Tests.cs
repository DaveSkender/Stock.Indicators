namespace StaticSeries;

[TestClass]
public class Rsi : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<RsiResult> results = Quotes
            .ToRsi();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Rsi != null));

        // sample values
        RsiResult r1 = results[13];
        Assert.AreEqual(null, r1.Rsi);

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
        IReadOnlyList<RsiResult> results = Quotes
            .ToRsi(lookbackPeriods);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.Rsi != null));

        // sample values
        RsiResult r1 = results[28];
        Assert.AreEqual(100, r1.Rsi);

        RsiResult r2 = results[52];
        Assert.AreEqual(0, r2.Rsi);
    }

    [TestMethod]
    public void CryptoData()
    {
        IReadOnlyList<Quote> btc = Data.GetBitcoin();

        IReadOnlyList<RsiResult> r = btc
            .ToRsi(1);

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<RsiResult> results = Quotes
            .Use(CandlePart.Close)
            .ToRsi();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Rsi != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<RsiResult> results = Quotes
            .ToSma(2)
            .ToRsi();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(487, results.Count(x => x.Rsi != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToRsi()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(479, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void NaN()
    {
        IReadOnlyList<RsiResult> r = Data.GetBtcUsdNan()
            .ToRsi();

        Assert.AreEqual(0, r.Count(x => x.Rsi is double.NaN));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<RsiResult> r = BadQuotes
            .ToRsi(20);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Rsi is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<RsiResult> r0 = Noquotes
            .ToRsi();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<RsiResult> r1 = Onequote
            .ToRsi();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<RsiResult> results = Quotes
            .ToRsi()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (10 * 14), results.Count);

        RsiResult last = results[^1];
        Assert.AreEqual(42.0773, last.Rsi.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.ToRsi(0));
}
