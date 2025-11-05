namespace StaticSeries;

[TestClass]
public class Smma : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<SmmaResult> results = Quotes
            .ToSmma(20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Smma != null));

        // starting calculations at proper index
        Assert.IsNull(results[18].Smma);
        Assert.IsNotNull(results[19].Smma);

        // sample values
        Assert.AreEqual(214.52500, Math.Round(results[19].Smma.Value, 5));
        Assert.AreEqual(214.55125, Math.Round(results[20].Smma.Value, 5));
        Assert.AreEqual(214.58319, Math.Round(results[21].Smma.Value, 5));
        Assert.AreEqual(225.78071, Math.Round(results[100].Smma.Value, 5));
        Assert.AreEqual(255.67462, Math.Round(results[501].Smma.Value, 5));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<SmmaResult> results = Quotes
            .Use(CandlePart.Close)
            .ToSmma(20);

        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Smma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<SmmaResult> results = Quotes
            .ToSma(2)
            .ToSmma(20);

        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Smma != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToSmma(20)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(474, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<SmmaResult> r = BadQuotes
            .ToSmma(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Smma is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<SmmaResult> r0 = Noquotes
            .ToSmma(5);

        Assert.IsEmpty(r0);

        IReadOnlyList<SmmaResult> r1 = Onequote
            .ToSmma(5);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<SmmaResult> results = Quotes
            .ToSmma(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - (20 + 100), results);
        Assert.AreEqual(255.67462, Math.Round(results[^1].Smma.Value, 5));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToSmma(0));
}
