namespace Tests.Indicators;

[TestClass, TestCategory("Integration")]
public class WilliamsRTests
{
    [TestMethod]
    public async Task Issue1127()
    {
        // initialize
        IEnumerable<Quote> feedQuotes = await FeedData  // live quotes
            .GetQuotes("A", 365 * 3)
            .ConfigureAwait(false);

        List<Quote> quotes = feedQuotes.ToList();
        int length = quotes.Count;

        // get indicators
        IReadOnlyList<WilliamsResult> resultsList = quotes
            .ToWilliamsR(14);

        Console.WriteLine($"%R from {length} quotes.");

        // analyze boundary
        for (int i = 0; i < length; i++)
        {
            Quote q = quotes[i];
            WilliamsResult r = resultsList[i];

            Console.WriteLine($"{q.Timestamp:s} {r.WilliamsR}");

            if (r.WilliamsR is not null)
            {
                Assert.IsTrue(r.WilliamsR <= 0);
                Assert.IsTrue(r.WilliamsR >= -100);
            }
        }
    }
}
