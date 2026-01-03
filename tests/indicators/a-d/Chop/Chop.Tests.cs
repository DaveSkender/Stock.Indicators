namespace Tests.Indicators;

[TestClass]
public class ChopTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<ChopResult> results = quotes
            .GetChop(14)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(488, results.Count(static x => x.Chop != null));

        // sample values
        ChopResult r1 = results[13];
        Assert.IsNull(r1.Chop);

        ChopResult r2 = results[14];
        Assert.AreEqual(69.9967, r2.Chop.Round(4));

        ChopResult r3 = results[249];
        Assert.AreEqual(41.8499, r3.Chop.Round(4));

        ChopResult r4 = results[501];
        Assert.AreEqual(38.6526, r4.Chop.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetChop(14)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(479, results.Count(static x => x.Sma != null));
    }

    [TestMethod]
    public void SmallLookback()
    {
        int lookbackPeriods = 2;
        List<ChopResult> results = quotes
            .GetChop(lookbackPeriods)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(500, results.Count(static x => x.Chop != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<ChopResult> r = badQuotes
            .GetChop(20)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Chop is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<ChopResult> r0 = noquotes
            .GetChop()
            .ToList();

        Assert.IsEmpty(r0);

        List<ChopResult> r1 = onequote
            .GetChop()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<ChopResult> results = quotes
            .GetChop(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 14, results);

        ChopResult last = results.LastOrDefault();
        Assert.AreEqual(38.6526, last.Chop.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetChop(1));
}
