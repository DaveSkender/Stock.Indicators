using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Stc : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int cyclePeriods = 9;
        int fastPeriods = 12;
        int slowPeriods = 26;

        List<StcResult> results =
            quotes.GetStc(cyclePeriods, fastPeriods, slowPeriods)
            .ToList();

        foreach (StcResult r in results)
        {
            Console.WriteLine($"{r.Date:d},{r.Stc:N4}");
        }

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(467, results.Count(x => x.Stc != null));

        // sample values
        StcResult r34 = results[34];
        Assert.IsNull(r34.Stc);

        StcResult r35 = results[35];
        Assert.AreEqual(100d, r35.Stc);

        StcResult r49 = results[49];
        Assert.AreEqual(0.8370, NullMath.Round(r49.Stc, 4));

        StcResult r249 = results[249];
        Assert.AreEqual(27.7340, NullMath.Round(r249.Stc, 4));

        StcResult last = results.LastOrDefault();
        Assert.AreEqual(19.2544, NullMath.Round(last.Stc, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<StcResult> results = quotes
            .Use(CandlePart.Close)
            .GetStc(9, 12, 26);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(467, results.Count(x => x.Stc != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<StcResult> r = tupleNanny.GetStc();

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Stc is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<StcResult> results = quotes
            .GetSma(2)
            .GetStc(9, 12, 26);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(466, results.Count(x => x.Stc != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetStc(9, 12, 26)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(458, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<StcResult> r = badQuotes.GetStc(10, 23, 50);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Stc is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<StcResult> r0 = noquotes.GetStc();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<StcResult> r1 = onequote.GetStc();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        int cyclePeriods = 9;
        int fastPeriods = 12;
        int slowPeriods = 26;

        List<StcResult> results =
            quotes.GetStc(cyclePeriods, fastPeriods, slowPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (slowPeriods + cyclePeriods + 250), results.Count);

        StcResult last = results.LastOrDefault();
        Assert.AreEqual(19.2544, NullMath.Round(last.Stc, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStc(quotes, 9, 0, 26));

        // bad slow periods must be larger than faster period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStc(quotes, 9, 12, 12));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStc(quotes, -1, 12, 26));
    }
}
