namespace StreamHubs;

[TestClass]
public class AlmaHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        AlmaHub observer = quoteHub.ToAlmaHub(10, 0.85, 6);

        // fetch initial results (early)
        IReadOnlyList<AlmaResult> sut = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
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

        IReadOnlyList<AlmaResult> expectedOriginal = Quotes.ToAlma(10, 0.85, 6);
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<AlmaResult> expectedRevised = RevisedQuotes.ToAlma(10, 0.85, 6);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int almaPeriods = 12;
        const int smaPeriods = 8;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        AlmaHub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToAlmaHub(almaPeriods, 0.85, 6);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<AlmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<AlmaResult> seriesList
           = quotesList
            .ToSma(smaPeriods)
            .ToAlma(almaPeriods, 0.85, 6);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int almaPeriods = 20;
        const int smaPeriods = 10;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize ALMA observer as quoteHub
        AlmaHub almaObserver = quoteHub
            .ToAlmaHub(almaPeriods, 0.85, 6);

        // initialize SMA observer
        SmaHub smaObserver = almaObserver
            .ToSmaHub(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++)
        {
            if (i == 80) { continue; }  // Skip for late arrival

            Quote q = Quotes[i];
            quoteHub.Add(q);

            if (i is > 100 and < 105) { quoteHub.Add(q); }  // Duplicate quotes
        }

        quoteHub.Insert(Quotes[80]);  // Late arrival
        quoteHub.Remove(Quotes[removeAtIndex]);  // Remove

        // final results
        IReadOnlyList<SmaResult> sut = smaObserver.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> expected = RevisedQuotes
            .ToAlma(almaPeriods, 0.85, 6)
            .ToSma(smaPeriods);

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        almaObserver.Unsubscribe();
        smaObserver.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        AlmaHub observer = quoteHub.ToAlmaHub(14, 0.85, 6);

        observer.ToString().Should().Be("ALMA(14,0.85,6)");

        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void AlmaHubParameters()
    {
        // Test various parameter combinations
        (int lookback, double offset, double sigma)[] parameters =
        [
            (lookback: 5, offset: 0.85, sigma: 6.0),
            (lookback: 10, offset: 0.5, sigma: 4.0),
            (lookback: 14, offset: 0.9, sigma: 8.0),
            (lookback: 20, offset: 0.25, sigma: 3.0)
        ];

        foreach ((int lookback, double offset, double sigma) in parameters)
        {
            // setup quote provider hub
            QuoteHub quoteHub = new();

            // initialize observer
            AlmaHub observer = quoteHub
                .ToAlmaHub(lookback, offset, sigma);

            // emulate quote stream
            for (int i = 0; i < Quotes.Count; i++)
            {
                quoteHub.Add(Quotes[i]);
            }

            // final results
            IReadOnlyList<AlmaResult> streamList = observer.Results;

            // time-series, for comparison
            IReadOnlyList<AlmaResult> seriesList = Quotes.ToAlma(lookback, offset, sigma);

            // assert, should equal series
            streamList.Should().HaveCount(Quotes.Count,
                $"Count mismatch for parameters: lookback={lookback}, offset={offset}, sigma={sigma}");
            streamList.IsExactly(seriesList,
                $"Results mismatch for parameters: lookback={lookback}, offset={offset}, sigma={sigma}");

            // cleanup
            observer.Unsubscribe();
            quoteHub.EndTransmission();
        }
    }

    [TestMethod]
    public void Reset()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with sample parameters
        AlmaHub observer = quoteHub.ToAlmaHub(14, 0.85, 6);

        // Add ~50 quotes to populate state
        for (int i = 0; i < 50; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // assert observer.Results has 50 entries and the last result has a non-null Alma value
        observer.Results.Should().HaveCount(50);
        observer.Results[^1].Alma.Should().NotBeNull();

        // call observer.Reinitialize() - this resets the subscription and rebuilds from quoteHub
        observer.Reinitialize();

        // The observer should still have 50 results since it rebuilds from the quoteHub
        observer.Results.Should().HaveCount(50);

        // Now test with a completely fresh setup after unsubscribing
        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();

        // Create a new quoteHub with just one quote
        QuoteHub freshProvider = new();
        AlmaHub freshObserver = freshProvider.ToAlmaHub(14, 0.85, 6);

        // Add one quote and assert observer.Results has count 1 and that the single result's Alma is null (since lookback period is 14)
        freshProvider.Add(Quotes[0]);

        freshObserver.Results.Should().HaveCount(1);
        freshObserver.Results[^1].Alma.Should().BeNull();

        // cleanup
        freshObserver.Unsubscribe();
        freshProvider.EndTransmission();
    }

    [TestMethod]
    public void AlmaHubExceptions()
    {
        QuoteHub quoteHub = new();

        // test constructor validation
        Action act1 = () => quoteHub.ToAlmaHub(1, 0.85, 6);
        act1.Should().Throw<ArgumentOutOfRangeException>("Lookback periods must be greater than 1");

        Action act2 = () => quoteHub.ToAlmaHub(0, 0.85, 6);
        act2.Should().Throw<ArgumentOutOfRangeException>("Lookback periods must be greater than 1");

        Action act3 = () => quoteHub.ToAlmaHub(-1, 0.85, 6);
        act3.Should().Throw<ArgumentOutOfRangeException>("Lookback periods must be greater than 1");

        Action act4 = () => quoteHub.ToAlmaHub(10, 1.1, 6);
        act4.Should().Throw<ArgumentOutOfRangeException>("Offset must be between 0 and 1");

        Action act5 = () => quoteHub.ToAlmaHub(10, -0.1, 6);
        act5.Should().Throw<ArgumentOutOfRangeException>("Offset must be between 0 and 1");

        Action act6 = () => quoteHub.ToAlmaHub(10, 0.85, 0);
        act6.Should().Throw<ArgumentOutOfRangeException>("Sigma must be greater than 0");

        Action act7 = () => quoteHub.ToAlmaHub(10, 0.85, -1);
        act7.Should().Throw<ArgumentOutOfRangeException>("Sigma must be greater than 0");

        quoteHub.EndTransmission();
    }
}
