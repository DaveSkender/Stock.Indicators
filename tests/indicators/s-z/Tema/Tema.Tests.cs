namespace Tests.Indicators;

[TestClass]
public class TemaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<TemaResult> results = quotes
            .GetTema(20)
            .ToList();

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
    public void UseTuple()
    {
        List<TemaResult> results = quotes
            .Use(CandlePart.Close)
            .GetTema(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Tema != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<TemaResult> r = tupleNanny
            .GetTema(6)
            .ToList();

        Assert.HasCount(200, r);
        Assert.IsEmpty(r.Where(static x => x.Tema is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        List<TemaResult> results = quotes
            .GetSma(2)
            .GetTema(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Tema != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetTema(20)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(474, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<TemaResult> r = badQuotes
            .GetTema(15)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Tema is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<TemaResult> r0 = noquotes
            .GetTema(5)
            .ToList();

        Assert.IsEmpty(r0);

        List<TemaResult> r1 = onequote
            .GetTema(5)
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<TemaResult> results = quotes
            .GetTema(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - ((3 * 20) + 100), results);

        TemaResult last = results.LastOrDefault();
        Assert.AreEqual(238.7690, last.Tema.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetTema(0));
}
