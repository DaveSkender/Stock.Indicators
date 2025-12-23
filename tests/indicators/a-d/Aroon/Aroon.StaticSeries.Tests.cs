namespace StaticSeries;

[TestClass]
public class Aroon : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<AroonResult> results = Quotes
            .ToAroon();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(477, results.Where(static x => x.AroonUp != null));
        Assert.HasCount(477, results.Where(static x => x.AroonDown != null));
        Assert.HasCount(477, results.Where(static x => x.Oscillator != null));

        // sample values
        AroonResult r1 = results[210];
        Assert.AreEqual(100, r1.AroonUp);
        Assert.AreEqual(000, r1.AroonDown);
        Assert.AreEqual(100, r1.Oscillator);

        AroonResult r2 = results[293];
        Assert.AreEqual(0, r2.AroonUp);
        Assert.AreEqual(40, r2.AroonDown);
        Assert.AreEqual(-40, r2.Oscillator);

        AroonResult r3 = results[298];
        Assert.AreEqual(0, r3.AroonUp);
        Assert.AreEqual(20, r3.AroonDown);
        Assert.AreEqual(-20, r3.Oscillator);

        AroonResult r4 = results[458];
        Assert.AreEqual(0, r4.AroonUp);
        Assert.AreEqual(100, r4.AroonDown);
        Assert.AreEqual(-100, r4.Oscillator);

        AroonResult r5 = results[501];
        Assert.AreEqual(28, r5.AroonUp);
        Assert.AreEqual(88, r5.AroonDown);
        Assert.AreEqual(-60, r5.Oscillator);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToAroon()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(468, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<AroonResult> r = BadQuotes
            .ToAroon(20);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Oscillator is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<AroonResult> r0 = Noquotes
            .ToAroon();

        Assert.IsEmpty(r0);

        IReadOnlyList<AroonResult> r1 = Onequote
            .ToAroon();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<AroonResult> results = Quotes
            .ToAroon()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 25, results);

        AroonResult last = results[^1];
        Assert.AreEqual(28, last.AroonUp);
        Assert.AreEqual(88, last.AroonDown);
        Assert.AreEqual(-60, last.Oscillator);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToAroon(0));
}
