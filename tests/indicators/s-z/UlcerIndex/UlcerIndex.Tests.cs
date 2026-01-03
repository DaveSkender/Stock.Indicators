namespace Tests.Indicators;

[TestClass]
public class UlcerIndexTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<UlcerIndexResult> results = quotes
            .GetUlcerIndex(14)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(489, results.Count(static x => x.UI != null));

        // sample value
        UlcerIndexResult r = results[501];
        Assert.AreEqual(5.7255, r.UI.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<UlcerIndexResult> results = quotes
            .Use(CandlePart.Close)
            .GetUlcerIndex(14)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(489, results.Count(static x => x.UI != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<UlcerIndexResult> r = tupleNanny
            .GetUlcerIndex(6)
            .ToList();

        Assert.HasCount(200, r);
        Assert.IsEmpty(r.Where(static x => x.UI is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        List<UlcerIndexResult> results = quotes
            .GetSma(2)
            .GetUlcerIndex(14)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(488, results.Count(static x => x.UI != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetUlcerIndex(14)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(480, results.Count(static x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<UlcerIndexResult> r = badQuotes
            .GetUlcerIndex(15)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.UI is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<UlcerIndexResult> r0 = noquotes
            .GetUlcerIndex()
            .ToList();

        Assert.IsEmpty(r0);

        List<UlcerIndexResult> r1 = onequote
            .GetUlcerIndex()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<UlcerIndexResult> results = quotes
            .GetUlcerIndex(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 13, results);

        UlcerIndexResult last = results.LastOrDefault();
        Assert.AreEqual(5.7255, last.UI.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetUlcerIndex(0));
}
