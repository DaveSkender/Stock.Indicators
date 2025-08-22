namespace Tests.Indicators;

[TestClass]
public class StcTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int cyclePeriods = 9;
        int fastPeriods = 12;
        int slowPeriods = 26;

        List<StcResult> results =
            quotes.GetStc(cyclePeriods, fastPeriods, slowPeriods)
            .ToList();

        foreach (StcResult r in results)
        {
            Console.WriteLine($"{r.Date:d},{r.Stc:N4}");
        }

        // proper quantities
        Assert.HasCount(502, results);
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
    public void UseTuple()
    {
        List<StcResult> results = quotes
            .Use(CandlePart.Close)
            .GetStc(9, 12, 26)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(467, results.Count(x => x.Stc != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<StcResult> r = tupleNanny
            .GetStc()
            .ToList();

        Assert.HasCount(200, r);
        Assert.AreEqual(0, r.Count(x => x.Stc is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<StcResult> results = quotes
            .GetSma(2)
            .GetStc(9, 12, 26)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(466, results.Count(x => x.Stc != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetStc(9, 12, 26)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(458, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<StcResult> r = badQuotes
            .GetStc(10, 23, 50)
            .ToList();

        Assert.HasCount(502, r);
        Assert.AreEqual(0, r.Count(x => x.Stc is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<StcResult> r0 = noquotes
            .GetStc()
            .ToList();

        Assert.IsEmpty(r0);

        List<StcResult> r1 = onequote
            .GetStc()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Issue1107()
    {
        // stochastic SMMA variant initialization bug

        RandomGbm quotes = new(58);

        List<StcResult> results = quotes
            .GetStc(10, 23, 50)
            .ToList();

        Assert.HasCount(58, results);
    }

    [TestMethod]
    public void Removed()
    {
        int cyclePeriods = 9;
        int fastPeriods = 12;
        int slowPeriods = 26;

        List<StcResult> results = quotes
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
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetStc(9, 0, 26));

        // bad slow periods must be larger than faster period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetStc(9, 12, 12));

        // bad signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetStc(-1, 12, 26));
    }
}
