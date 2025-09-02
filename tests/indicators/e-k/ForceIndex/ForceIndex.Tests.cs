namespace Tests.Indicators;

[TestClass]
public class ForceIndexTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<ForceIndexResult> r = quotes.GetForceIndex(13).ToList();

        // proper quantities
        Assert.HasCount(502, r);
        Assert.AreEqual(489, r.Count(x => x.ForceIndex != null));

        // sample values
        Assert.IsNull(r[12].ForceIndex);

        Assert.AreEqual(10668240.778, Math.Round(r[13].ForceIndex.Value, 3));
        Assert.AreEqual(15883211.364, Math.Round(r[24].ForceIndex.Value, 3));
        Assert.AreEqual(7598218.196, Math.Round(r[149].ForceIndex.Value, 3));
        Assert.AreEqual(23612118.994, Math.Round(r[249].ForceIndex.Value, 3));
        Assert.AreEqual(-16824018.428, Math.Round(r[501].ForceIndex.Value, 3));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetForceIndex(13)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(480, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<ForceIndexResult> r = badQuotes
            .GetForceIndex(2)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.ForceIndex is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<ForceIndexResult> r0 = noquotes
            .GetForceIndex(5)
            .ToList();

        Assert.IsEmpty(r0);

        List<ForceIndexResult> r1 = onequote
            .GetForceIndex(5)
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<ForceIndexResult> results = quotes
            .GetForceIndex(13)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - (13 + 100), results);

        ForceIndexResult last = results.LastOrDefault();
        Assert.AreEqual(-16824018.428, Math.Round(last.ForceIndex.Value, 3));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetForceIndex(0));
}
