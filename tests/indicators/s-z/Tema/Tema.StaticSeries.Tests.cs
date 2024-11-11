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
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Tema != null));

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

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Tema != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<TemaResult> results = Quotes
            .ToSma(2)
            .ToTema(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Tema != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToTema(20)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<TemaResult> r = BadQuotes
            .ToTema(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Tema is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<TemaResult> r0 = Noquotes
            .ToTema(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<TemaResult> r1 = Onequote
            .ToTema(5);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<TemaResult> results = Quotes
            .ToTema(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (3 * 20 + 100), results.Count);

        TemaResult last = results[^1];
        Assert.AreEqual(238.7690, last.Tema.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.ToTema(0));
}
