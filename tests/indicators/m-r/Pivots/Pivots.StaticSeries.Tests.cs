namespace StaticSeries;

[TestClass]
public class Pivots : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<PivotsResult> results = Quotes
            .ToPivots(4, 4);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(35, results.Where(x => x.HighPoint != null));
        Assert.HasCount(333, results.Where(x => x.HighTrend != null));
        Assert.HasCount(338, results.Where(x => x.HighLine != null));
        Assert.HasCount(34, results.Where(x => x.LowPoint != null));
        Assert.HasCount(328, results.Where(x => x.LowTrend != null));
        Assert.HasCount(333, results.Where(x => x.LowLine != null));

        // sample values
        PivotsResult r3 = results[3];
        Assert.IsNull(r3.HighPoint);
        Assert.IsNull(r3.HighTrend);
        Assert.IsNull(r3.HighLine);
        Assert.IsNull(r3.LowPoint);
        Assert.IsNull(r3.LowTrend);
        Assert.IsNull(r3.LowLine);

        PivotsResult r7 = results[7];
        Assert.IsNull(r7.HighPoint);
        Assert.IsNull(r7.HighTrend);
        Assert.IsNull(r7.HighLine);
        Assert.AreEqual(212.53m, r7.LowPoint);
        Assert.IsNull(r7.LowTrend);
        Assert.AreEqual(212.53m, r7.LowLine);

        PivotsResult r120 = results[120];
        Assert.AreEqual(233.02m, r120.HighPoint);
        Assert.AreEqual(PivotTrend.Lh, r120.HighTrend);
        Assert.AreEqual(233.02m, r120.HighLine);
        Assert.IsNull(r120.LowPoint);
        Assert.AreEqual(PivotTrend.Ll, r120.LowTrend);
        Assert.AreEqual(228.9671m, r120.LowLine.Round(4));

        PivotsResult r180 = results[180];
        Assert.AreEqual(239.74m, r180.HighPoint);
        Assert.AreEqual(PivotTrend.Hh, r180.HighTrend);
        Assert.AreEqual(239.74m, r180.HighLine);
        Assert.IsNull(r180.LowPoint);
        Assert.AreEqual(PivotTrend.Hl, r180.LowTrend);
        Assert.AreEqual(236.7050m, r180.LowLine.Round(4));

        PivotsResult r250 = results[250];
        Assert.IsNull(r250.HighPoint);
        Assert.IsNull(r250.HighTrend);
        Assert.IsNull(r250.HighLine);
        Assert.AreEqual(256.81m, r250.LowPoint);
        Assert.IsNull(r250.LowTrend);
        Assert.IsNull(r250.LowLine);

        PivotsResult r472 = results[472];
        Assert.IsNull(r472.HighPoint);
        Assert.AreEqual(PivotTrend.Lh, r472.HighTrend);
        Assert.AreEqual(274.14m, r472.HighLine);
        Assert.IsNull(r472.LowPoint);
        Assert.AreEqual(PivotTrend.Hl, r472.LowTrend);
        Assert.AreEqual(255.8078m, r472.LowLine.Round(4));

        PivotsResult r497 = results[497];
        Assert.IsNull(r497.HighPoint);
        Assert.IsNull(r497.HighTrend);
        Assert.IsNull(r497.HighLine);
        Assert.IsNull(r497.LowPoint);
        Assert.IsNull(r497.LowTrend);
        Assert.IsNull(r497.LowLine);

        PivotsResult r498 = results[498];
        Assert.IsNull(r498.HighPoint);
        Assert.IsNull(r498.HighTrend);
        Assert.IsNull(r498.HighLine);
        Assert.IsNull(r498.LowPoint);
        Assert.IsNull(r498.LowTrend);
        Assert.IsNull(r498.LowLine);
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<PivotsResult> r = BadQuotes
            .ToPivots();

        Assert.HasCount(502, r);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<PivotsResult> r0 = Noquotes
            .ToPivots();

        Assert.IsEmpty(r0);

        IReadOnlyList<PivotsResult> r1 = Onequote
            .ToPivots();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<PivotsResult> results = Quotes
            .ToPivots(4, 4)
            .Condense();
        Assert.HasCount(67, results);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad left span
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToPivots(1));

        // bad right span
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToPivots(2, 1));

        // bad lookback window
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToPivots(20, 10, 20, EndType.Close));
    }
}
