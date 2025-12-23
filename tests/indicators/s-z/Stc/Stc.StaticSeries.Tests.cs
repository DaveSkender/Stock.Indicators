namespace StaticSeries;

[TestClass]
public class Stc : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int cyclePeriods = 9;
        const int fastPeriods = 12;
        const int slowPeriods = 26;

        IReadOnlyList<StcResult> results = Quotes
            .ToStc(cyclePeriods, fastPeriods, slowPeriods);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(467, results.Where(static x => x.Stc != null));

        // sample values
        StcResult r34 = results[34];
        Assert.IsNull(r34.Stc);

        StcResult r35 = results[35];
        Assert.AreEqual(100d, r35.Stc);

        StcResult r49 = results[49];
        Assert.AreEqual(0.8370, r49.Stc.Round(4));

        StcResult r249 = results[249];
        Assert.AreEqual(27.7340, r249.Stc.Round(4));

        StcResult last = results[^1];
        Assert.AreEqual(19.2544, last.Stc.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<StcResult> results = Quotes
            .Use(CandlePart.Close)
            .ToStc(9, 12, 26);

        Assert.HasCount(502, results);
        Assert.HasCount(467, results.Where(static x => x.Stc != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<StcResult> results = Quotes
            .ToSma(2)
            .ToStc(9, 12, 26);

        Assert.HasCount(502, results);
        Assert.HasCount(466, results.Where(static x => x.Stc != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToStc(9, 12, 26)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(458, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<StcResult> r = BadQuotes
            .ToStc();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Stc is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<StcResult> r0 = Noquotes
            .ToStc();

        Assert.IsEmpty(r0);

        IReadOnlyList<StcResult> r1 = Onequote
            .ToStc();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Issue1107_Magic58_IsNotOutOfRange()
    {
        // stochastic SMMA variant initialization bug

        RandomGbm quotes = new(58);

        IReadOnlyList<StcResult> results = quotes
            .ToStc();

        Assert.HasCount(58, results);
    }

    [TestMethod]
    public void Removed()
    {
        const int cyclePeriods = 9;
        const int fastPeriods = 12;
        const int slowPeriods = 26;

        IReadOnlyList<StcResult> results = Quotes
            .ToStc(cyclePeriods, fastPeriods, slowPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - (slowPeriods + cyclePeriods + 250), results);

        StcResult last = results[^1];
        Assert.AreEqual(19.2544, last.Stc.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStc(9, 0, 26));

        // bad slow periods must be larger than faster period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStc(9, 12, 12));

        // bad signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStc(-1, 12, 26));
    }
}
