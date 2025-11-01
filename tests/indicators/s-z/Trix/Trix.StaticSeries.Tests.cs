namespace StaticSeries;

[TestClass]
public class Trix : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<TrixResult> results = Quotes
            .ToTrix(20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Ema3 != null));
        Assert.HasCount(482, results.Where(static x => x.Trix != null));

        // sample values
        TrixResult r24 = results[24];
        Assert.AreEqual(214.5486, r24.Ema3.Round(4));
        Assert.AreEqual(0.005047, r24.Trix.Round(6));

        TrixResult r67 = results[67];
        Assert.AreEqual(221.7837, r67.Ema3.Round(4));
        Assert.AreEqual(0.050030, r67.Trix.Round(6));

        TrixResult r249 = results[249];
        Assert.AreEqual(249.4469, r249.Ema3.Round(4));
        Assert.AreEqual(0.121781, r249.Trix.Round(6));

        TrixResult r501 = results[501];
        Assert.AreEqual(263.3216, r501.Ema3.Round(4));
        Assert.AreEqual(-0.230742, r501.Trix.Round(6));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<TrixResult> results = Quotes
            .Use(CandlePart.Close)
            .ToTrix(20);

        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Trix != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<TrixResult> results = Quotes
            .ToSma(2)
            .ToTrix(20);

        Assert.HasCount(502, results);
        Assert.HasCount(481, results.Where(static x => x.Trix != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToTrix(20)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(473, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<TrixResult> r = BadQuotes
            .ToTrix(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Trix is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<TrixResult> r0 = Noquotes
            .ToTrix(5);

        Assert.IsEmpty(r0);

        IReadOnlyList<TrixResult> r1 = Onequote
            .ToTrix(5);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<TrixResult> results = Quotes
            .ToTrix(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - ((3 * 20) + 100), results);

        TrixResult last = results[^1];
        Assert.AreEqual(263.3216, last.Ema3.Round(4));
        Assert.AreEqual(-0.230742, last.Trix.Round(6));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToTrix(0));
}
