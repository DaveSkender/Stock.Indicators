namespace StreamHub;

[TestClass]
public class StdDevChannelsHubTests : StreamHubTestBase, ITestChainObserver
{
    private const int lookbackPeriods = 20;
    private const double standardDeviations = 2;
    private readonly IReadOnlyList<StdDevChannelsResult> expectedOriginal = Quotes.ToStdDevChannels(lookbackPeriods, standardDeviations);

    [TestMethod]
    public void QuoteObserver()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        StdDevChannelsHub observer = quoteHub.ToStdDevChannelsHub(lookbackPeriods, standardDeviations);

        // fetch initial results (early)
        IReadOnlyList<StdDevChannelsResult> actuals = observer.Results;

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

        IReadOnlyList<StdDevChannelsResult> expectedRevised = RevisedQuotes.ToStdDevChannels(lookbackPeriods, standardDeviations);

        actuals.Should().HaveCount(501);
        actuals.Should().BeEquivalentTo(expectedRevised, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        const int emaPeriods = 12;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        StdDevChannelsHub observer = quoteHub
            .ToEmaHub(emaPeriods)
            .ToStdDevChannelsHub(lookbackPeriods, standardDeviations);

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<StdDevChannelsResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<StdDevChannelsResult> expected = Quotes
            .ToEma(emaPeriods)
            .ToStdDevChannels(lookbackPeriods, standardDeviations);

        // assert, should equal series
        actuals.Should().HaveCount(length);
        actuals.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        StdDevChannelsHub hub = new(new QuoteHub(), 20, 2);
        hub.ToString().Should().Be("STDEV-CHANNELS(20,2)");
    }
}
