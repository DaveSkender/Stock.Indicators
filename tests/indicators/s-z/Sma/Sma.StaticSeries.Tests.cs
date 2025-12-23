namespace StaticSeries;

[TestClass]
public partial class Sma : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToSma(20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Sma != null));

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
            .ToSma(20);

        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Sma != null));

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
            .ToSma(20);

        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Sma != null));

        // sample values
        SmaResult r24 = results[24];
        Assert.AreEqual(77293768.2, r24.Sma);

        SmaResult r290 = results[290];
        Assert.AreEqual(157958070.8, r290.Sma);

        SmaResult r501 = results[501];
        Assert.AreEqual(DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", invariantCulture), r501.Timestamp);
        Assert.AreEqual(163695200, r501.Sma);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .ToSma(10)
            .ToEma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(484, results.Where(static x => x.Ema != null));
    }

    [TestMethod]
    public void NaN()
    {
        IReadOnlyList<SmaResult> r = Data.GetBtcUsdNan()
            .ToSma(50);

        Assert.IsEmpty(r.Where(static x => x.Sma is double.NaN));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<SmaResult> r = BadQuotes
            .ToSma(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Sma is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<SmaResult> r0 = Noquotes
            .ToSma(5);

        Assert.IsEmpty(r0);

        IReadOnlyList<SmaResult> r1 = Onequote
            .ToSma(5);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToSma(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 19, results);
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

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToSma(0));
}
