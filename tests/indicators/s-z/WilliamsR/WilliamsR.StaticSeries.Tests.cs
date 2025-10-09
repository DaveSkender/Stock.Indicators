using Test.Data;

namespace StaticSeries;

[TestClass]
public class WilliamsR : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<WilliamsResult> results = Quotes
            .ToWilliamsR();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(489, results.Where(x => x.WilliamsR != null));

        // sample values
        WilliamsResult r1 = results[343];
        Assert.AreEqual(-19.8211, r1.WilliamsR.Round(4));

        WilliamsResult r2 = results[501];
        Assert.AreEqual(-52.0121, r2.WilliamsR.Round(4));

        // test boundary condition
        for (int i = 0; i < results.Count; i++)
        {
            WilliamsResult r = results[i];

            r.WilliamsR?.Should().BeInRange(-100d, 0d);
        }
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToWilliamsR()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(480, results.Where(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<WilliamsResult> results = BadQuotes
            .ToWilliamsR(20);

        Assert.HasCount(502, results);
        Assert.IsEmpty(results.Where(x => x.WilliamsR is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<WilliamsResult> r0 = Noquotes
            .ToWilliamsR();

        Assert.IsEmpty(r0);

        IReadOnlyList<WilliamsResult> r1 = Onequote
            .ToWilliamsR();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<WilliamsResult> results = Quotes
            .ToWilliamsR()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 13, results);

        WilliamsResult last = results[^1];
        Assert.AreEqual(-52.0121, last.WilliamsR.Round(4));
    }

    [TestMethod]
    public void Boundary()
    {
        IReadOnlyList<WilliamsResult> results = Data
            .GetRandom(2500)
            .ToWilliamsR();

        // analyze boundary
        for (int i = 0; i < results.Count; i++)
        {
            WilliamsResult r = results[i];

            r.WilliamsR?.Should().BeInRange(-100d, 0d);
        }
    }

    [TestMethod]
    public void Issue1127()
    {
        // initialize
        IOrderedEnumerable<Quote> test1127 = File.ReadAllLines("s-z/WilliamsR/issue1127quotes.csv")
            .Skip(1)
            .Select(Test.Data.Convert.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp);

        IReadOnlyList<Quote> quotesList = test1127.ToList();
        int length = quotesList.Count;

        // get indicators
        IReadOnlyList<WilliamsResult> resultsList = quotesList
            .ToWilliamsR();

        Console.WriteLine($"%R from {length} quotes.");

        // analyze boundary
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            WilliamsResult r = resultsList[i];

            Console.WriteLine($"{q.Timestamp:s} {r.WilliamsR}");

            r.WilliamsR?.Should().BeInRange(-100d, 0d);
        }
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToWilliamsR(0));
}
