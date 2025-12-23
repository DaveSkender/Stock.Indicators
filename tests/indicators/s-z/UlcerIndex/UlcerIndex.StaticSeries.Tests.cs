namespace StaticSeries;

[TestClass]
public class UlcerIndex : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<UlcerIndexResult> results = Quotes
            .ToUlcerIndex();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(489, results.Where(static x => x.UlcerIndex != null));

        // sample value
        UlcerIndexResult r = results[501];
        Assert.AreEqual(5.7255, r.UlcerIndex.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<UlcerIndexResult> results = Quotes
            .Use(CandlePart.Close)
            .ToUlcerIndex();

        Assert.HasCount(502, results);
        Assert.HasCount(489, results.Where(static x => x.UlcerIndex != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<UlcerIndexResult> results = Quotes
            .ToSma(2)
            .ToUlcerIndex();

        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(static x => x.UlcerIndex != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToUlcerIndex()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(480, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<UlcerIndexResult> r = BadQuotes
            .ToUlcerIndex(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.UlcerIndex is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<UlcerIndexResult> r0 = Noquotes
            .ToUlcerIndex();

        Assert.IsEmpty(r0);

        IReadOnlyList<UlcerIndexResult> r1 = Onequote
            .ToUlcerIndex();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<UlcerIndexResult> results = Quotes
            .ToUlcerIndex()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 13, results);

        UlcerIndexResult last = results[^1];
        Assert.AreEqual(5.7255, last.UlcerIndex.Round(4));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToUlcerIndex(0));
}
