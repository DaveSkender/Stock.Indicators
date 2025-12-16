namespace StaticSeries;

[TestClass]
public class Cmo : StaticSeriesTestBase
{
    // TODO: test for CMO isUp works as expected
    // when there’s no price change

    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<CmoResult> results = Quotes
            .ToCmo(14);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(static x => x.Cmo != null));

        // sample values
        CmoResult r13 = results[13];
        Assert.IsNull(r13.Cmo);

        CmoResult r14 = results[14];
        Assert.AreEqual(24.1081, r14.Cmo.Round(4));

        CmoResult r249 = results[249];
        Assert.AreEqual(48.9614, r249.Cmo.Round(4));

        CmoResult r501 = results[501];
        Assert.AreEqual(-26.7502, r501.Cmo.Round(4));
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<CmoResult> results = Quotes.ToCmo(14);

        TestAsserts.AlwaysBounded(results, static x => x.Cmo, -100d, 100d);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<CmoResult> results = Quotes
            .Use(CandlePart.Close)
            .ToCmo(14);

        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(static x => x.Cmo != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<CmoResult> results = Quotes
            .ToSma(2)
            .ToCmo(20);

        Assert.HasCount(502, results);
        Assert.HasCount(481, results.Where(static x => x.Cmo != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToCmo(20)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(473, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<CmoResult> r = BadQuotes
            .ToCmo(35);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Cmo is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<CmoResult> r0 = Noquotes
            .ToCmo(5);

        Assert.IsEmpty(r0);

        IReadOnlyList<CmoResult> r1 = Onequote
            .ToCmo(5);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<CmoResult> results = Quotes
            .ToCmo(14)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(488, results);

        CmoResult last = results[^1];
        Assert.AreEqual(-26.7502, last.Cmo.Round(4));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToCmo(0));
}
