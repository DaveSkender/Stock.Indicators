namespace Series;

[TestClass]
public class WilliamsRTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<WilliamsResult> results = Quotes
            .GetWilliamsR();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Count(x => x.WilliamsR != null));

        // sample values
        WilliamsResult r1 = results[343];
        Assert.AreEqual(-19.8211, r1.WilliamsR.Round(4));

        WilliamsResult r2 = results[501];
        Assert.AreEqual(-52.0121, r2.WilliamsR.Round(4));

        // test boundary condition
        for (int i = 0; i < results.Count; i++)
        {
            WilliamsResult r = results[i];

            if (r.WilliamsR is not null)
            {
                Assert.IsTrue(r.WilliamsR <= 0);
                Assert.IsTrue(r.WilliamsR >= -100);
            }
        }
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetWilliamsR()
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<WilliamsResult> results = BadQuotes
            .GetWilliamsR(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(0, results.Count(x => x.WilliamsR is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<WilliamsResult> r0 = Noquotes
            .GetWilliamsR();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<WilliamsResult> r1 = Onequote
            .GetWilliamsR();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<WilliamsResult> results = Quotes
            .GetWilliamsR()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 13, results.Count);

        WilliamsResult last = results[^1];
        Assert.AreEqual(-52.0121, last.WilliamsR.Round(4));
    }

    [TestMethod]
    public void Boundary()
    {
        IReadOnlyList<WilliamsResult> results = Data
            .GetRandom(2500)
            .GetWilliamsR();

        // analyze boundary
        for (int i = 0; i < results.Count; i++)
        {
            WilliamsResult r = results[i];

            if (r.WilliamsR is not null)
            {
                Assert.IsTrue(r.WilliamsR <= 0);
                Assert.IsTrue(r.WilliamsR >= -100);
            }
        }
    }

    [TestMethod]
    public void Issue1127()
    {
        // initialize
        IOrderedEnumerable<Quote> test1127 = File.ReadAllLines("s-z/WilliamsR/issue1127quotes.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp);

        IReadOnlyList<Quote> quotesList = test1127.ToList();
        int length = quotesList.Count;

        // get indicators
        IReadOnlyList<WilliamsResult> resultsList = quotesList
            .GetWilliamsR();

        Console.WriteLine($"%R from {length} quotes.");

        // analyze boundary
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            WilliamsResult r = resultsList[i];

            Console.WriteLine($"{q.Timestamp:s} {r.WilliamsR}");

            if (r.WilliamsR is not null)
            {
                Assert.IsTrue(r.WilliamsR <= 0);
                Assert.IsTrue(r.WilliamsR >= -100);
            }
        }
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetWilliamsR(0));
}
