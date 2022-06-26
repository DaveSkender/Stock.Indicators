using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Wma : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<WmaResult> results = quotes.GetWma(20).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Wma != null).Count());

        // sample values
        WmaResult r1 = results[149];
        Assert.AreEqual(235.5253, NullMath.Round(r1.Wma, 4));

        WmaResult r2 = results[501];
        Assert.AreEqual(246.5110, NullMath.Round(r2.Wma, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<WmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetWma(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(483, results.Where(x => x.Wma != null).Count());
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<WmaResult> results = quotes
            .GetSma(1)
            .GetWma(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(483, results.Where(x => x.Wma != null).Count());
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetWma(20)
            .GetSma(10);

        Assert.AreEqual(483, results.Count());
        Assert.AreEqual(474, results.Where(x => x.Sma != null).Count());
    }

    [TestMethod]
    public void Chaining()
    {
        List<WmaResult> standard = quotes
            .GetWma(17)
            .ToList();

        List<WmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetWma(17)
            .ToList();

        // assertions
        for (int i = 0; i < results.Count; i++)
        {
            WmaResult s = standard[i];
            WmaResult c = results[i];

            Assert.AreEqual(s.Date, c.Date);
            Assert.AreEqual(s.Wma, c.Wma);
        }
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<WmaResult> r = Indicator.GetWma(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<WmaResult> r0 = noquotes.GetWma(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<WmaResult> r1 = onequote.GetWma(5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<WmaResult> results = quotes.GetWma(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        WmaResult last = results.LastOrDefault();
        Assert.AreEqual(246.5110, NullMath.Round(last.Wma, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetWma(quotes, 0));
}
