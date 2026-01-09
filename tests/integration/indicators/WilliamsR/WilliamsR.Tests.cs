using Test.Tools;

namespace Tests.Indicators;

[TestClass, TestCategory("Integration")]
public class WilliamsRTests
{
    [TestMethod]
    public async Task ToWilliamsR_LiveData_MaintainsBoundaryRange()
    {
        // initialize
        IEnumerable<Quote> feedQuotes = await FeedData  // live quotes
            .GetQuotes("A", 365 * 3)
            .ConfigureAwait(false);

        List<Quote> quotes = feedQuotes.ToList();

        // get indicators
        IReadOnlyList<WilliamsResult> results = quotes
            .ToWilliamsR(14);

        results.ToConsole(static r => r.WilliamsR is > 0d or < -100d, (nameof(WilliamsResult.WilliamsR), "F20"));

        results.IsBetween(static results => results.WilliamsR, -100d, 0d);
    }
}
