namespace StreamHub;

[TestClass]
public class AlmaHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 20; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        AlmaHub<Quote> observer = provider
            .ToAlma(10, 0.85, 6);

        // fetch initial results (early)
        IReadOnlyList<AlmaResult> streamList
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            provider.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                provider.Add(q);
            }
        }

        // late arrival
        provider.Insert(quotesList[80]);

        // delete
        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<AlmaResult> seriesList = quotesList.ToAlma(10, 0.85, 6);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        int almaPeriods = 12;
        int smaPeriods = 8;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        AlmaHub<SmaResult> observer = provider
            .ToSma(smaPeriods)
            .ToAlma(almaPeriods, 0.85, 6);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
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
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        int almaPeriods = 20;
        int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize ALMA observer as provider
        AlmaHub<Quote> almaObserver = provider
            .ToAlma(almaPeriods, 0.85, 6);

        // initialize SMA observer
        SmaHub<AlmaResult> smaObserver = almaObserver
            .ToSma(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
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
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> provider = new();
        AlmaHub<Quote> observer = provider.ToAlma(14, 0.85, 6);

        observer.ToString().Should().Be("ALMA(14,0.85,6)");

        provider.EndTransmission();
    }

    [TestMethod]
    public void AlmaHubParameters()
    {
        // Test various parameter combinations
        var parameters = new[]
        {
            (lookback: 5, offset: 0.85, sigma: 6.0),
            (lookback: 10, offset: 0.5, sigma: 4.0),
            (lookback: 14, offset: 0.9, sigma: 8.0),
            (lookback: 20, offset: 0.25, sigma: 3.0)
        };

        List<Quote> quotesList = Quotes.ToList();

        foreach (var (lookback, offset, sigma) in parameters)
        {
            // setup quote provider
            QuoteHub<Quote> provider = new();

            // initialize observer
            AlmaHub<Quote> observer = provider
                .ToAlma(lookback, offset, sigma);

            // emulate quote stream
            for (int i = 0; i < quotesList.Count; i++)
            {
                provider.Add(quotesList[i]);
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
            provider.EndTransmission();
        }
    }

    [TestMethod]
    public void Reset()
    {
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer with sample parameters
        AlmaHub<Quote> observer = provider.ToAlma(14, 0.85, 6);

        // Add ~50 quotes to populate state
        for (int i = 0; i < 50; i++)
        {
            provider.Add(quotesList[i]);
        }

        // assert observer.Results has 50 entries and the last result has a non-null Alma value
        observer.Results.Should().HaveCount(50);
        observer.Results[^1].Alma.Should().NotBeNull();

        // call observer.Reinitialize()
        observer.Reinitialize();

        // Add one more quote and assert observer.Results has count 1 and that the single result's Alma is null (or uninitialized)
        provider.Add(quotesList[50]);

        observer.Results.Should().HaveCount(1);
        observer.Results[^1].Alma.Should().BeNull();

        // cleanup
        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void AlmaHubExceptions()
    {
        QuoteHub<Quote> provider = new();

        // test constructor validation
        Action act1 = () => provider.ToAlma(1, 0.85, 6);
        act1.Should().Throw<ArgumentOutOfRangeException>("Lookback periods must be greater than 1");

        Action act2 = () => provider.ToAlma(0, 0.85, 6);
        act2.Should().Throw<ArgumentOutOfRangeException>("Lookback periods must be greater than 1");

        Action act3 = () => provider.ToAlma(-1, 0.85, 6);
        act3.Should().Throw<ArgumentOutOfRangeException>("Lookback periods must be greater than 1");

        Action act4 = () => provider.ToAlma(10, 1.1, 6);
        act4.Should().Throw<ArgumentOutOfRangeException>("Offset must be between 0 and 1");

        Action act5 = () => provider.ToAlma(10, -0.1, 6);
        act5.Should().Throw<ArgumentOutOfRangeException>("Offset must be between 0 and 1");

        Action act6 = () => provider.ToAlma(10, 0.85, 0);
        act6.Should().Throw<ArgumentOutOfRangeException>("Sigma must be greater than 0");

        Action act7 = () => provider.ToAlma(10, 0.85, -1);
        act7.Should().Throw<ArgumentOutOfRangeException>("Sigma must be greater than 0");

        provider.EndTransmission();
    }
}
