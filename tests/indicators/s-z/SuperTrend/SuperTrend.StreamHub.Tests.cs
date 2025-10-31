namespace StreamHub;

[TestClass]
public class SuperTrendHubTests : StreamHubTestBase, ITestQuoteObserver
{
    private const int lookbackPeriods = 14;
    private const double multiplier = 3;
    private readonly IReadOnlyList<SuperTrendResult> expectedOriginal
        = Quotes.ToSuperTrend(lookbackPeriods, multiplier);

    [TestMethod]
    public void QuoteObserver()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        SuperTrendHub observer = quoteHub.ToSuperTrendHub(lookbackPeriods, multiplier);

        // fetch initial results (early)
        IReadOnlyList<SuperTrendResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival, should equal series
        quoteHub.Insert(Quotes[80]);
        actuals.Should().BeEquivalentTo(expectedOriginal, static options => options.WithStrictOrdering());

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<SuperTrendResult> expectedRevised = RevisedQuotes.ToSuperTrend(lookbackPeriods, multiplier);

        actuals.Should().HaveCount(501);
        actuals.Should().BeEquivalentTo(expectedRevised, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        SuperTrendHub hub = new(new QuoteHub(), 14, 3.0);
        hub.ToString().Should().Be("SUPERTREND(14,3)");
    }
}
