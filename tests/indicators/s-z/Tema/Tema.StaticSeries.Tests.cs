namespace StaticSeries;

[TestClass]
public class Tema : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<TemaResult> results = Quotes
            .ToTema(20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Tema != null));

        // sample values
        TemaResult r25 = results[25];
        Assert.AreEqual(216.1441, r25.Tema.Round(4));

        TemaResult r67 = results[67];
        Assert.AreEqual(222.9562, r67.Tema.Round(4));

        TemaResult r249 = results[249];
        Assert.AreEqual(258.6208, r249.Tema.Round(4));

        TemaResult r501 = results[501];
        Assert.AreEqual(238.7690, r501.Tema.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<TemaResult> results = Quotes
            .Use(CandlePart.Close)
            .ToTema(20);

        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Tema != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<TemaResult> results = Quotes
            .ToSma(2)
            .ToTema(20);

        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Tema != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToTema(20)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(474, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<TemaResult> r = BadQuotes
            .ToTema(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Tema is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<TemaResult> r0 = Noquotes
            .ToTema(5);

        Assert.IsEmpty(r0);

        IReadOnlyList<TemaResult> r1 = Onequote
            .ToTema(5);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<TemaResult> results = Quotes
            .ToTema(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - ((3 * 20) + 100), results);

        TemaResult last = results[^1];
        Assert.AreEqual(238.7690, last.Tema.Round(4));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToTema(0));
}
