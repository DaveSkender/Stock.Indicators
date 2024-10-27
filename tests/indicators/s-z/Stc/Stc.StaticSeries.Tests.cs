namespace StaticSeries;

[TestClass]
public class Stc : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int cyclePeriods = 9;
        int fastPeriods = 12;
        int slowPeriods = 26;

        IReadOnlyList<StcResult> results = Quotes
            .ToStc(cyclePeriods, fastPeriods, slowPeriods);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(467, results.Count(x => x.Stc != null));

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

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(467, results.Count(x => x.Stc != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<StcResult> results = Quotes
            .ToSma(2)
            .ToStc(9, 12, 26);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(466, results.Count(x => x.Stc != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToStc(9, 12, 26)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(458, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<StcResult> r = BadQuotes
            .ToStc();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Stc is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<StcResult> r0 = Noquotes
            .ToStc();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<StcResult> r1 = Onequote
            .ToStc();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Issue1107()
    {
        // stochastic SMMA variant initialization bug

        RandomGbm quotes = new(58);

        IReadOnlyList<StcResult> results = quotes
            .ToStc();

        Assert.AreEqual(58, results.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int cyclePeriods = 9;
        int fastPeriods = 12;
        int slowPeriods = 26;

        IReadOnlyList<StcResult> results = Quotes
            .ToStc(cyclePeriods, fastPeriods, slowPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (slowPeriods + cyclePeriods + 250), results.Count);

        StcResult last = results[^1];
        Assert.AreEqual(19.2544, last.Stc.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.ToStc(9, 0, 26));

        // bad slow periods must be larger than faster period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.ToStc(9, 12, 12));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.ToStc(-1, 12, 26));
    }
}
