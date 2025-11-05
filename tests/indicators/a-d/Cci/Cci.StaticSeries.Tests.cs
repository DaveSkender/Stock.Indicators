namespace StaticSeries;

[TestClass]
public class Cci : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<CciResult> results = Quotes
            .ToCci();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Cci != null));

        // sample value
        CciResult r = results[501];
        Assert.AreEqual(-52.9946, r.Cci.Round(4));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToCci()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(474, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<CciResult> r = BadQuotes
            .ToCci(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Cci is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<CciResult> r0 = Noquotes
            .ToCci();

        Assert.IsEmpty(r0);

        IReadOnlyList<CciResult> r1 = Onequote
            .ToCci();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<CciResult> results = Quotes
            .ToCci()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 19, results);

        CciResult last = results[^1];
        Assert.AreEqual(-52.9946, last.Cci.Round(4));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToCci(0));
}
