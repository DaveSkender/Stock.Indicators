namespace StaticSeries;

[TestClass]
public class Sma : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetSma(20);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample values
        Assert.IsNull(results[18].Sma);
        Assert.AreEqual(214.5250, results[19].Sma.Round(4));
        Assert.AreEqual(215.0310, results[24].Sma.Round(4));
        Assert.AreEqual(234.9350, results[149].Sma.Round(4));
        Assert.AreEqual(255.5500, results[249].Sma.Round(4));
        Assert.AreEqual(251.8600, results[501].Sma.Round(4));
    }

    [TestMethod]
    public void CandlePartOpen()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .Use(CandlePart.Open)
            .GetSma(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample values
        Assert.IsNull(results[18].Sma);
        Assert.AreEqual(214.3795, results[19].Sma.Round(4));
        Assert.AreEqual(214.9535, results[24].Sma.Round(4));
        Assert.AreEqual(234.8280, results[149].Sma.Round(4));
        Assert.AreEqual(255.6915, results[249].Sma.Round(4));
        Assert.AreEqual(253.1725, results[501].Sma.Round(4));
    }

    [TestMethod]
    public void CandlePartVolume()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .Use(CandlePart.Volume)
            .GetSma(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample values
        SmaResult r24 = results[24];
        Assert.AreEqual(77293768.2, r24.Sma);

        SmaResult r290 = results[290];
        Assert.AreEqual(157958070.8, r290.Sma);

        SmaResult r501 = results[501];
        Assert.AreEqual(DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture), r501.Timestamp);
        Assert.AreEqual(163695200, r501.Sma);
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .GetSma(10)
            .GetEma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(484, results.Count(x => x.Ema != null));
    }

    [TestMethod]
    public void NaN()
    {
        IReadOnlyList<SmaResult> r = Data.GetBtcUsdNan()
            .GetSma(50);

        Assert.AreEqual(0, r.Count(x => x.Sma is double.NaN));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<SmaResult> r = BadQuotes
            .GetSma(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Sma is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<SmaResult> r0 = Noquotes
            .GetSma(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<SmaResult> r1 = Onequote
            .GetSma(5);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetSma(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);
        Assert.AreEqual(251.8600, results[^1].Sma.Round(4));
    }

    [TestMethod]
    public void Equality()
    {
        SmaResult r1 = new(Timestamp: EvalDate, Sma: 1d);

        SmaResult r2 = new(Timestamp: EvalDate, Sma: 1d);

        SmaResult r3 = new(Timestamp: EvalDate, Sma: 2d);

        Assert.IsTrue(Equals(r1, r2));
        Assert.IsFalse(Equals(r1, r3));

        Assert.IsTrue(r1.Equals(r2));
        Assert.IsFalse(r1.Equals(r3));

        Assert.IsTrue(r1 == r2);
        Assert.IsFalse(r1 == r3);

        Assert.IsFalse(r1 != r2);
        Assert.IsTrue(r1 != r3);
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetSma(0));
}
