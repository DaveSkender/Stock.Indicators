namespace StaticSeries;

[TestClass]
public class Fractal : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard() // Span 2
    {
        IReadOnlyList<FractalResult> results = Quotes
            .ToFractal();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(63, results.Where(x => x.FractalBear != null));
        Assert.HasCount(71, results.Where(x => x.FractalBull != null));

        // sample values
        FractalResult r1 = results[1];
        Assert.IsNull(r1.FractalBear);
        Assert.IsNull(r1.FractalBull);

        FractalResult r2 = results[3];
        Assert.AreEqual(215.17m, r2.FractalBear);
        Assert.IsNull(r2.FractalBull);

        FractalResult r3 = results[133];
        Assert.AreEqual(234.53m, r3.FractalBear);
        Assert.IsNull(r3.FractalBull);

        FractalResult r4 = results[180];
        Assert.AreEqual(239.74m, r4.FractalBear);
        Assert.AreEqual(238.52m, r4.FractalBull);

        FractalResult r5 = results[250];
        Assert.IsNull(r5.FractalBear);
        Assert.AreEqual(256.81m, r5.FractalBull);

        FractalResult r6 = results[500];
        Assert.IsNull(r6.FractalBear);
        Assert.IsNull(r6.FractalBull);
    }

    [TestMethod]
    public void StandardSpan4()
    {
        IReadOnlyList<FractalResult> results = Quotes
            .ToFractal(4, 4);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(35, results.Count(x => x.FractalBear != null));
        Assert.AreEqual(34, results.Count(x => x.FractalBull != null));

        // sample values
        FractalResult r1 = results[3];
        Assert.IsNull(r1.FractalBear);
        Assert.IsNull(r1.FractalBull);

        FractalResult r2 = results[7];
        Assert.IsNull(r2.FractalBear);
        Assert.AreEqual(212.53m, r2.FractalBull);

        FractalResult r3 = results[120];
        Assert.AreEqual(233.02m, r3.FractalBear);
        Assert.IsNull(r3.FractalBull);

        FractalResult r4 = results[180];
        Assert.AreEqual(239.74m, r4.FractalBear);
        Assert.IsNull(r4.FractalBull);

        FractalResult r5 = results[250];
        Assert.IsNull(r5.FractalBear);
        Assert.AreEqual(256.81m, r5.FractalBull);

        FractalResult r6 = results[500];
        Assert.IsNull(r6.FractalBear);
        Assert.IsNull(r6.FractalBull);
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<FractalResult> r = BadQuotes
            .ToFractal();

        Assert.HasCount(502, r);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<FractalResult> r0 = Noquotes
            .ToFractal();

        Assert.IsEmpty(r0);

        IReadOnlyList<FractalResult> r1 = Onequote
            .ToFractal();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<FractalResult> results = Quotes
            .ToFractal()
            .Condense();

        Assert.HasCount(129, results);
    }

    // bad window span
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToFractal(1));
}
