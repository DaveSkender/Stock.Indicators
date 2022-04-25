using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Tema : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<TemaResult> results = quotes.GetTema(20).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Tema != null).Count());

        // sample values
        TemaResult r25 = results[25];
        Assert.AreEqual(216.1441m, Math.Round((decimal)r25.Tema, 4));

        TemaResult r67 = results[67];
        Assert.AreEqual(222.9562m, Math.Round((decimal)r67.Tema, 4));

        TemaResult r249 = results[249];
        Assert.AreEqual(258.6208m, Math.Round((decimal)r249.Tema, 4));

        TemaResult r501 = results[501];
        Assert.AreEqual(238.7690m, Math.Round((decimal)r501.Tema, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<TemaResult> r = Indicator.GetTema(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<TemaResult> r0 = noquotes.GetTema(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<TemaResult> r1 = onequote.GetTema(5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<TemaResult> results = quotes.GetTema(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - ((3 * 20) + 100), results.Count);

        TemaResult last = results.LastOrDefault();
        Assert.AreEqual(238.7690m, Math.Round((decimal)last.Tema, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetTema(quotes, 0));
    }
}
