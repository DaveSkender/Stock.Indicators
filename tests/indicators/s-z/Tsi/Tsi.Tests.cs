namespace Tests.Indicators;

[TestClass]
public class TsiTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<TsiResult> results = quotes
            .GetTsi(25, 13, 7)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(465, results.Count(x => x.Tsi != null));
        Assert.AreEqual(459, results.Count(x => x.Signal != null));

        // sample values
        TsiResult r2 = results[37];
        Assert.AreEqual(53.1204, r2.Tsi.Round(4));
        Assert.IsNull(r2.Signal);

        TsiResult r3a = results[43];
        Assert.AreEqual(46.0960, r3a.Tsi.Round(4));
        Assert.AreEqual(51.6916, r3a.Signal.Round(4));

        TsiResult r3b = results[44];
        Assert.AreEqual(42.5121, r3b.Tsi.Round(4));
        Assert.AreEqual(49.3967, r3b.Signal.Round(4));

        TsiResult r4 = results[149];
        Assert.AreEqual(29.0936, r4.Tsi.Round(4));
        Assert.AreEqual(28.0134, r4.Signal.Round(4));

        TsiResult r5 = results[249];
        Assert.AreEqual(41.9232, r5.Tsi.Round(4));
        Assert.AreEqual(42.4063, r5.Signal.Round(4));

        TsiResult r6 = results[501];
        Assert.AreEqual(-28.3513, r6.Tsi.Round(4));
        Assert.AreEqual(-29.3597, r6.Signal.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<TsiResult> results = quotes
            .Use(CandlePart.Close)
            .GetTsi()
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(465, results.Count(x => x.Tsi != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<TsiResult> r = tupleNanny
            .GetTsi()
            .ToList();

        Assert.HasCount(200, r);
        Assert.AreEqual(0, r.Count(x => x.Tsi is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        List<TsiResult> results = quotes
            .GetSma(2)
            .GetTsi()
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(464, results.Count(x => x.Tsi != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetTsi()
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(456, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<TsiResult> r = badQuotes
            .GetTsi()
            .ToList();

        Assert.HasCount(502, r);
        Assert.AreEqual(0, r.Count(x => x.Tsi is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void BigData()
    {
        List<TsiResult> r = bigQuotes
            .GetTsi()
            .ToList();

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<TsiResult> r0 = noquotes
            .GetTsi()
            .ToList();

        Assert.IsEmpty(r0);

        List<TsiResult> r1 = onequote
            .GetTsi()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<TsiResult> results = quotes
            .GetTsi(25, 13, 7)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - (25 + 13 + 250), results);

        TsiResult last = results.LastOrDefault();
        Assert.AreEqual(-28.3513, last.Tsi.Round(4));
        Assert.AreEqual(-29.3597, last.Signal.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetTsi(0));

        // bad smoothing period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetTsi(25, 0));

        // bad signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetTsi(25, 13, -1));
    }
}
