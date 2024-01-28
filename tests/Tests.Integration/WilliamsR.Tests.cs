using Skender.Stock.Indicators;

namespace Tests.Indicators;

[TestClass]
public class WilliamsRTests
{
    [TestMethod]
    public async Task Issue1127()
    {
        // initialize
        IEnumerable<Quote> quotes = await FeedData  // live quotes
            .GetQuotes("A", 365 * 3)
            .ConfigureAwait(false);

        List<Quote> quotesList = quotes.ToList();
        int length = quotesList.Count;

        // get indicators
        List<WilliamsResult> resultsList = quotes
            .GetWilliamsR(14)
            .ToList();

        Console.WriteLine($"%R from {length} quotes.");

        // analyze boundary
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            WilliamsResult r = resultsList[i];

            Console.WriteLine($"{q.Date:s} {r.WilliamsR}");

            if (r.WilliamsR is not null)
            {
                Assert.IsTrue(r.WilliamsR <= 0);
                Assert.IsTrue(r.WilliamsR >= -100);
            }
        }
    }
}
