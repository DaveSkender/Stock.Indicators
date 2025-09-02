namespace Tests.Indicators;

[TestClass]
public class WilliamsRTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<WilliamsResult> results = quotes
            .GetWilliamsR(14)
            .ToList();

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
        List<SmaResult> results = quotes
            .GetWilliamsR()
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(480, results.Where(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<Quote> quotes = badQuotes
            .ToSortedList();

        List<WilliamsResult> results = badQuotes
            .GetWilliamsR(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.IsEmpty(results.Where(x => x.WilliamsR is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<WilliamsResult> r0 = noquotes
            .GetWilliamsR()
            .ToList();

        Assert.IsEmpty(r0);

        List<WilliamsResult> r1 = onequote
            .GetWilliamsR()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<WilliamsResult> results = quotes
            .GetWilliamsR(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 13, results);

        WilliamsResult last = results.LastOrDefault();
        Assert.AreEqual(-52.0121, last.WilliamsR.Round(4));
    }

    [TestMethod]
    public void Boundary()
    {
        List<WilliamsResult> results = TestData
            .GetRandom(2500)
            .GetWilliamsR(14)
            .ToList();

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
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Date);

        List<Quote> quotesList = test1127.ToList();
        int length = quotesList.Count;

        // get indicators
        List<WilliamsResult> resultsList = test1127
            .GetWilliamsR(14)
            .ToList();

        Console.WriteLine($"%R from {length} quotes.");

        // analyze boundary
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            WilliamsResult r = resultsList[i];

            Console.WriteLine($"{q.Date:s} {r.WilliamsR}");

            r.WilliamsR?.Should().BeInRange(-100d, 0d);
        }
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetWilliamsR(0));
}
