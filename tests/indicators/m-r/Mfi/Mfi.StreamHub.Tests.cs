namespace StreamHub;

[TestClass]
public class MfiHub : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    private const int lookbackPeriods = 14;
    private static readonly IReadOnlyList<MfiResult> expectedOriginal = Quotes.ToMfi(lookbackPeriods);

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        MfiResult[] results = [.. Quotes.ToMfiHub(14).Results];
        TestAssert.IsBetween(results, x => x.Mfi, 0, 100);
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        Skender.Stock.Indicators.MfiHub observer = quoteHub.ToMfiHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<MfiResult> actuals = observer.Results;

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

        IReadOnlyList<MfiResult> expectedRevised = RevisedQuotes.ToMfi(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.Should().BeEquivalentTo(expectedRevised, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        // MFI emits IReusable results (MfiResult implements IReusable with Value = Mfi),
        // so it can act as a chain provider for downstream indicators.

        const int mfiPeriods = 14;
        const int emaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize chain: MFI then EMA over its Value
        EmaHub observer = quoteHub
            .ToMfiHub(mfiPeriods)
            .ToEmaHub(emaPeriods);

        // stream quotes
        foreach (Quote q in quotesList)
        {
            quoteHub.Add(q);
        }

        // results from stream
        IReadOnlyList<EmaResult> streamList = observer.Results;

        // time-series parity
        IReadOnlyList<EmaResult> seriesList = quotesList
            .ToMfi(mfiPeriods)
            .ToEma(emaPeriods);

        streamList.Should().HaveCount(seriesList.Count);
        streamList.Should().BeEquivalentTo(seriesList, static o => o.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();

        Skender.Stock.Indicators.MfiHub hub = new(quoteHub, lookbackPeriods);
        hub.ToString().Should().Be($"MFI({lookbackPeriods})");
    }
}
