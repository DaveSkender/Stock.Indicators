namespace StreamHub;

[TestClass]
public class DpoHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 14;
    private readonly IReadOnlyList<DpoResult> expectedOriginal = Quotes.ToDpo(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        DpoHub observer = quoteHub.ToDpoHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<DpoResult> actuals = observer.Results;

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

        IReadOnlyList<DpoResult> expectedRevised = RevisedQuotes.ToDpo(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.Should().BeEquivalentTo(expectedRevised, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int dpoPeriods = 14;
        const int smaPeriods = 8;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        DpoHub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToDpoHub(dpoPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<DpoResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<DpoResult> expected = Quotes
            .ToSma(smaPeriods)
            .ToDpo(dpoPeriods);

        // assert, should equal series
        actuals.Should().HaveCount(length);
        actuals.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int dpoPeriods = 14;
        const int smaPeriods = 10;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize DPO hub as provider
        DpoHub dpoHub = quoteHub
            .ToSmaHub(smaPeriods)
            .ToDpoHub(dpoPeriods);

        // initialize consumer (EMA of DPO)
        EmaHub emaHub = dpoHub.ToEmaHub(5);

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<EmaResult> actuals = emaHub.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> expected = Quotes
            .ToSma(smaPeriods)
            .ToDpo(dpoPeriods)
            .ToEma(5);

        // assert, should equal series
        actuals.Should().HaveCount(length);
        actuals.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());

        // cleanup
        emaHub.Unsubscribe();
        dpoHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        DpoHub hub = new(new QuoteHub(), 14);
        hub.ToString().Should().Be("DPO(14)");
    }
}
