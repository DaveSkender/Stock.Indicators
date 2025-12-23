using Test.Tools;

namespace Tests.Indicators;

[TestClass, TestCategory("Integration")]
public class WilliamsRTests
{
    [TestMethod]
    public async Task ToWilliamsR_WithLiveData_MaintainsBoundaryRange()
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

        results.ToConsole(r => r.WilliamsR is > 0d or < -100d, (nameof(WilliamsResult.WilliamsR), "F20"));

        // TODO: address rounding at boundaries (only)
        results.IsBetween(results => results.WilliamsR, -100d, 0d);
    }
}
