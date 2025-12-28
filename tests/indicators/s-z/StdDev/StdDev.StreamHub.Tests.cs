namespace StreamHubs;

[TestClass]
public class StdDevHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 10;
    private readonly IReadOnlyList<StdDevResult> expectedOriginal = Quotes.ToStdDev(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        StdDevHub observer = quoteHub.ToStdDevHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<StdDevResult> actuals = observer.Results;

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

        IReadOnlyList<StdDevResult> expectedRevised = RevisedQuotes.ToStdDev(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int stdDevPeriods = 10;
        const int smaPeriods = 8;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        StdDevHub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToStdDevHub(stdDevPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<StdDevResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<StdDevResult> expected = Quotes
            .ToSma(smaPeriods)
            .ToStdDev(stdDevPeriods);

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
        const int stdDevPeriods = 10;
        const int smaPeriods = 8;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToStdDevHub(stdDevPeriods)
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
        IReadOnlyList<SmaResult> actuals
            = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> seriesList = RevisedQuotes
            .ToStdDev(stdDevPeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        actuals.Should().HaveCount(length - 1);
        actuals.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        StdDevHub hub = new(new QuoteHub(), 14);
        hub.ToString().Should().Be("STDDEV(14)");
    }
}
