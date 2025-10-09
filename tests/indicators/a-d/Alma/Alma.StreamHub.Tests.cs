namespace StreamHub;

[TestClass]
public class AlmaHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        AlmaHub<Quote> observer = quoteHub
            .ToAlmaHub(10, 0.85, 6);

        // fetch initial results (early)
        IReadOnlyList<AlmaResult> streamList
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(quotesList[80]);

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<AlmaResult> seriesList = quotesList.ToAlma(10, 0.85, 6);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        const int almaPeriods = 12;
        const int smaPeriods = 8;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // initialize observer
        AlmaHub<SmaResult> observer = quoteHub
            .ToSma(smaPeriods)
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
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int almaPeriods = 20;
        const int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // initialize ALMA observer as quoteHub
        AlmaHub<Quote> almaObserver = quoteHub
            .ToAlmaHub(almaPeriods, 0.85, 6);

        // initialize SMA observer
        SmaHub<AlmaResult> smaObserver = almaObserver
            .ToSma(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<SmaResult> streamList
            = smaObserver.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList
           = quotesList
            .ToAlma(almaPeriods, 0.85, 6)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        almaObserver.Unsubscribe();
        smaObserver.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> quoteHub = new();
        AlmaHub<Quote> observer = quoteHub.ToAlmaHub(14, 0.85, 6);

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

        List<Quote> quotesList = Quotes.ToList();

        foreach ((int lookback, double offset, double sigma) in parameters)
        {
            // setup quote provider hub
            QuoteHub<Quote> quoteHub = new();

            // initialize observer
            AlmaHub<Quote> observer = quoteHub
                .ToAlmaHub(lookback, offset, sigma);

            // emulate quote stream
            for (int i = 0; i < quotesList.Count; i++)
            {
                quoteHub.Add(quotesList[i]);
            }

            // final results
            IReadOnlyList<AlmaResult> streamList = observer.Results;

            // time-series, for comparison
            IReadOnlyList<AlmaResult> seriesList = quotesList.ToAlma(lookback, offset, sigma);

            // assert, should equal series
            streamList.Should().HaveCount(quotesList.Count,
                $"Count mismatch for parameters: lookback={lookback}, offset={offset}, sigma={sigma}");
            streamList.Should().BeEquivalentTo(seriesList,
                $"Results mismatch for parameters: lookback={lookback}, offset={offset}, sigma={sigma}");

            observer.Unsubscribe();
            quoteHub.EndTransmission();
        }
    }

    [TestMethod]
    public void Reset()
    {
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // initialize observer with sample parameters
        AlmaHub<Quote> observer = quoteHub.ToAlmaHub(14, 0.85, 6);

        // Add ~50 quotes to populate state
        for (int i = 0; i < 50; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // assert observer.Results has 50 entries and the last result has a non-null Alma value
        observer.Results.Should().HaveCount(50);
        observer.Results[^1].Alma.Should().NotBeNull();

        // call observer.Reinitialize() - this resets the subscription and rebuilds from quoteHub
        observer.Reinitialize();

        // The observer should still have 50 results since it rebuilds from the quoteHub
        observer.Results.Should().HaveCount(50);

        // Now test with a completely fresh setup after unsubscribing
        observer.Unsubscribe();
        quoteHub.EndTransmission();

        // Create a new quoteHub with just one quote
        QuoteHub<Quote> freshProvider = new();
        AlmaHub<Quote> freshObserver = freshProvider.ToAlmaHub(14, 0.85, 6);

        // Add one quote and assert observer.Results has count 1 and that the single result's Alma is null (since lookback period is 14)
        freshProvider.Add(quotesList[0]);

        freshObserver.Results.Should().HaveCount(1);
        freshObserver.Results[^1].Alma.Should().BeNull();

        // cleanup
        freshObserver.Unsubscribe();
        freshProvider.EndTransmission();
    }

    [TestMethod]
    public void AlmaHubExceptions()
    {
        QuoteHub<Quote> quoteHub = new();

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
