namespace StaticSeries;

[TestClass]
public class Aroon : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<AroonResult> results = Quotes
            .ToAroon();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(477, results.Count(x => x.AroonUp != null));
        Assert.AreEqual(477, results.Count(x => x.AroonDown != null));
        Assert.AreEqual(477, results.Count(x => x.Oscillator != null));

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
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToAroon()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(468, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<AroonResult> r = BadQuotes
            .ToAroon(20);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Oscillator is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<AroonResult> r0 = Noquotes
            .ToAroon();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<AroonResult> r1 = Onequote
            .ToAroon();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<AroonResult> results = Quotes
            .ToAroon()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 25, results.Count);

        AroonResult last = results[^1];
        Assert.AreEqual(28, last.AroonUp);
        Assert.AreEqual(88, last.AroonDown);
        Assert.AreEqual(-60, last.Oscillator);
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.ToAroon(0));
}
