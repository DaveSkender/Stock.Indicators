namespace StreamHubs;

[TestClass]
public class StcHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int cyclePeriods = 9;
    private const int fastPeriods = 12;
    private const int slowPeriods = 26;
    private readonly IReadOnlyList<StcResult> expectedOriginal = Quotes.ToStc(cyclePeriods, fastPeriods, slowPeriods);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        StcHub observer = quoteHub.ToStcHub(cyclePeriods, fastPeriods, slowPeriods);

        // fetch initial results (early)
        IReadOnlyList<StcResult> actuals = observer.Results;

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
        actuals.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<StcResult> expectedRevised = RevisedQuotes.ToStc(cyclePeriods, fastPeriods, slowPeriods);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 8;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        StcHub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToStcHub(cyclePeriods, fastPeriods, slowPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<StcResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<StcResult> expected = Quotes
            .ToSma(smaPeriods)
            .ToStc(cyclePeriods, fastPeriods, slowPeriods);

        // assert, should equal series
        actuals.Should().HaveCount(length);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 10;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToStcHub(cyclePeriods, fastPeriods, slowPeriods)
            .ToSmaHub(smaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival
        quoteHub.Insert(Quotes[80]);

        // delete
        quoteHub.Remove(Quotes[removeAtIndex]);

        // final results
        IReadOnlyList<SmaResult> actuals = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> seriesList = RevisedQuotes
            .ToStc(cyclePeriods, fastPeriods, slowPeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        actuals.Should().HaveCount(length - 1);
        actuals.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<StcResult> sut = Quotes.ToStcHub(9, 12, 26).Results;
        sut.IsBetween(static x => x.Stc, 0, 100);
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyDefaults()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with default parameters (10, 23, 50)
        StcHub observer = quoteHub.ToStcHub();

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<StcResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<StcResult> expected = Quotes.ToStc();

        // assert, should equal series
        actuals.Should().HaveCount(length);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        StcHub hub = new(new QuoteHub(), cyclePeriods, fastPeriods, slowPeriods);
        hub.ToString().Should().Be($"STC({cyclePeriods},{fastPeriods},{slowPeriods})");
    }
}
