namespace Series;

[TestClass]
public class StcTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int cyclePeriods = 9;
        int fastPeriods = 12;
        int slowPeriods = 26;

        List<StcResult> results = Quotes
            .GetStc(cyclePeriods, fastPeriods, slowPeriods)
            .ToList();

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

        StcResult last = results.LastOrDefault();
        Assert.AreEqual(19.2544, last.Stc.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        List<StcResult> results = Quotes
            .Use(CandlePart.Close)
            .GetStc(9, 12, 26)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(467, results.Count(x => x.Stc != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<StcResult> results = Quotes
            .GetSma(2)
            .GetStc(9, 12, 26)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(466, results.Count(x => x.Stc != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetStc(9, 12, 26)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(458, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<StcResult> r = BadQuotes
            .GetStc()
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Stc is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<StcResult> r0 = Noquotes
            .GetStc()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<StcResult> r1 = Onequote
            .GetStc()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Issue1107()
    {
        // stochastic SMMA variant initialization bug

        RandomGbm quotes = new(58);

        List<StcResult> results = quotes
            .GetStc()
            .ToList();

        Assert.AreEqual(58, results.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int cyclePeriods = 9;
        int fastPeriods = 12;
        int slowPeriods = 26;

        List<StcResult> results = Quotes
            .GetStc(cyclePeriods, fastPeriods, slowPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (slowPeriods + cyclePeriods + 250), results.Count);

        StcResult last = results.LastOrDefault();
        Assert.AreEqual(19.2544, last.Stc.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetStc(9, 0, 26));

        // bad slow periods must be larger than faster period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetStc(9, 12, 12));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetStc(-1, 12, 26));
    }
}
