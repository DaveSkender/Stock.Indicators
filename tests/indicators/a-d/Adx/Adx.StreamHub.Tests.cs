namespace StreamHub;

[TestClass]
public class AdxHub : StreamHubTestBase, ITestChainProvider
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
        AdxHub<Quote> observer = provider
            .ToAdx(14);

        // fetch initial results (early)
        IReadOnlyList<AdxResult> streamList
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
        IReadOnlyList<AdxResult> seriesList = quotesList.ToAdx(14);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        int adxPeriods = 14;
        int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        SmaHub<AdxResult> observer = provider
            .ToAdx(adxPeriods)
            .ToSma(smaPeriods);

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
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

        // final results
        IReadOnlyList<SmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList
           = quotesList.ToAdx(adxPeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        AdxHub<Quote> hub = new(new QuoteHub<Quote>(), 14);
        hub.ToString().Should().Be("ADX(14)");
    }

    [TestMethod]
    public void StandardCalculations()
    {
        // Test with standard lookback period
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        AdxHub<Quote> observer = provider.ToAdx(14);

        // add all quotes
        foreach (Quote quote in quotesList)
        {
            provider.Add(quote);
        }

        // get results
        IReadOnlyList<AdxResult> streamList = observer.Results;
        IReadOnlyList<AdxResult> seriesList = quotesList.ToAdx(14);

        // assert equivalence
        streamList.Should().HaveCount(seriesList.Count);
        streamList.Should().BeEquivalentTo(seriesList);

        // test specific values at known indices
        AdxResult streamResult = streamList[501];
        AdxResult seriesResult = seriesList[501];

        streamResult.Pdi.Should().BeApproximately(seriesResult.Pdi!.Value, 1E-8);
        streamResult.Mdi.Should().BeApproximately(seriesResult.Mdi!.Value, 1E-8);
        streamResult.Dx.Should().BeApproximately(seriesResult.Dx!.Value, 1E-8);
        streamResult.Adx.Should().BeApproximately(seriesResult.Adx!.Value, 1E-8);

        if (seriesResult.Adxr.HasValue)
        {
            streamResult.Adxr.Should().BeApproximately(seriesResult.Adxr!.Value, 1E-8);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void DifferentPeriods()
    {
        // Test with different lookback periods
        List<Quote> quotesList = Quotes.ToList();
        int[] periods = [7, 10, 20, 25];

        foreach (int period in periods)
        {
            // setup quote provider
            QuoteHub<Quote> provider = new();

            // initialize observer
            AdxHub<Quote> observer = provider.ToAdx(period);

            // add all quotes
            foreach (Quote quote in quotesList)
            {
                provider.Add(quote);
            }

            // get results
            IReadOnlyList<AdxResult> streamList = observer.Results;
            IReadOnlyList<AdxResult> seriesList = quotesList.ToAdx(period);

            // assert equivalence
            streamList.Should().HaveCount(seriesList.Count);
            streamList.Should().BeEquivalentTo(seriesList);

            observer.Unsubscribe();
            provider.EndTransmission();
        }
    }

    [TestMethod]
    public void WarmupPeriods()
    {
        // Test that warmup periods are properly handled
        List<Quote> quotesList = Quotes.Take(50).ToList();

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        AdxHub<Quote> observer = provider.ToAdx(14);

        // add quotes one by one and check results
        for (int i = 0; i < quotesList.Count; i++)
        {
            provider.Add(quotesList[i]);

            IReadOnlyList<AdxResult> currentResults = observer.Results;
            AdxResult lastResult = currentResults.Last();

            // Check that early periods have null values
            if (i < 14)
            {
                lastResult.Pdi.Should().BeNull();
                lastResult.Mdi.Should().BeNull();
                lastResult.Dx.Should().BeNull();
                lastResult.Adx.Should().BeNull();
                lastResult.Adxr.Should().BeNull();
            }
            else if (i < 27) // 2 * 14 - 1
            {
                lastResult.Pdi.Should().NotBeNull();
                lastResult.Mdi.Should().NotBeNull();
                lastResult.Dx.Should().NotBeNull();
                lastResult.Adx.Should().BeNull(); // ADX not calculated yet
                lastResult.Adxr.Should().BeNull();
            }
            else
            {
                lastResult.Pdi.Should().NotBeNull();
                lastResult.Mdi.Should().NotBeNull();
                lastResult.Dx.Should().NotBeNull();
                lastResult.Adx.Should().NotBeNull();
                // ADXR may or may not be calculated depending on implementation
            }
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}