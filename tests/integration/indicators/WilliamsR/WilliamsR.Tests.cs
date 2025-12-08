using FluentAssertions;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

[TestClass, TestCategory("Integration")]
public class WilliamsRTests
{
    [TestMethod]
    public async Task LiveTest()
    {
        // initialize
        IEnumerable<Quote> feedQuotes = await FeedData  // live quotes
            .GetQuotes("A", 365 * 3)
            .ConfigureAwait(false);

        List<Quote> quotes = feedQuotes.ToList();
        int length = quotes.Count;

        // get indicators
        IReadOnlyList<WilliamsResult> results = quotes
            .ToWilliamsR(14);

        //Console.WriteLine(quotes.ToStringOut());
        //Console.WriteLine("----------------------------------------");
        Dictionary<string, string> args = new()
        {
            { "WilliamsR", "N20" }
        };

        Console.WriteLine(results.ToStringOut(args));
        Console.WriteLine("----------------------------------------");
        //Console.WriteLine(results.Where(x => x.WilliamsR is < (-100) or > 0).ToStringOut());

        // analyze boundary
        for (int i = 0; i < length; i++)
        {
            Quote q = quotes[i];
            WilliamsResult r = results[i];

            Console.WriteLine($"{q.Timestamp:s} {r.WilliamsR}");

            r.WilliamsR?.Should().BeInRange(-100d, 0d);  // TODO: address rounding at boundaries (only)
        }
    }
}
