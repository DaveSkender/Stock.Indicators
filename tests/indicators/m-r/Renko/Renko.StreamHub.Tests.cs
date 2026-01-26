namespace StreamHubs;

[TestClass]
public class RenkoHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    // NOTE: Renko transforms quotes to variable brick counts (non-1:1 timestamps).
    // Intentionally excluded from comprehensive provider history testing (Insert/Remove)
    // as quote transformations don't preserve timestamp mappings.
    // TODO: Revisit to explore alternative testing approach for quote transformations.

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        const decimal brickSize = 2.5m;
        const EndType endType = EndType.HighLow;

        List<Quote> quotes = Quotes.ToList();

        int length = quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 50; i++)
        {
            quoteHub.Add(quotes[i]);
        }

        // initialize observer
        RenkoHub observer = quoteHub
            .ToRenkoHub(brickSize, endType);

        // fetch initial results (early)
        IReadOnlyList<RenkoResult> streamList
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 50; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(quotes[80]);

        // delete
        quoteHub.RemoveAt(350);
        quotes.RemoveAt(350);

        // time-series, for comparison
        IReadOnlyList<RenkoResult> seriesList = quotes
            .ToRenko(brickSize, endType);

        // assert, should equal series
        streamList.IsExactly(seriesList);
        streamList.Should().HaveCount(159);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void WithCachePruning_MatchesSeriesExactly()
    {
        // NOTE: Renko transforms quotes to bricks (non-1:1 mapping).
        // The test validates that streaming with cache pruning produces
        // the same brick sequence as the full series, not that result count
        // matches cache size (which is invalid for transformation indicators).

        const int maxCacheSize = 100;  // Sufficient for quote retention
        const int totalQuotes = 200;  // ~2x cache size
        const decimal brickSize = 2.5m;
        const EndType endType = EndType.HighLow;

        IReadOnlyList<Quote> quotes = Quotes.Take(totalQuotes).ToList();
        
        // Get full series results
        IReadOnlyList<RenkoResult> fullSeries = quotes
            .ToRenko(brickSize, endType)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new() { MaxCacheSize = maxCacheSize };
        RenkoHub observer = quoteHub.ToRenkoHub(brickSize, endType);

        // Stream more quotes than cache can hold
        quoteHub.Add(quotes);

        // Verify cache was pruned (quotes, not results)
        quoteHub.Quotes.Should().HaveCount(maxCacheSize);

        // For Renko, validate the streaming results match the full series
        // The count will be much less than cache size due to brick transformation
        observer.Results.IsExactly(fullSeries);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const decimal brickSize = 2.5m;
        const EndType endType = EndType.Close;
        const int smaPeriods = 50;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToRenkoHub(brickSize, endType)
            .ToSmaHub(smaPeriods);

        // emulate quote stream (Renko transforms to bricks, no Insert/Remove)
        for (int i = 0; i < quotesCount; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // final results
        IReadOnlyList<SmaResult> sut = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> expected = Quotes
            .ToRenko(brickSize, endType)
            .ToSma(smaPeriods);

        // assert, should equal series
        sut.IsExactly(expected);
        sut.Should().HaveCount(112);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        RenkoHub hub = new(new QuoteHub(), 2.5m, EndType.Close);
        hub.ToString().Should().Be("RENKO(2.5,CLOSE)");
    }

    [TestMethod]
    public void SettingsInheritance()
    {
        // setup quote hub (1st level)
        QuoteHub quoteHub = new();

        // setup renko hub (2nd level)
        RenkoHub renkoHub = quoteHub
            .ToRenkoHub(brickSize: 2.5m, endType: EndType.Close);

        // setup child hub (3rd level)
        SmaHub childHub = renkoHub
            .ToSmaHub(lookbackPeriods: 5);

        // note: dispite `quoteHub` being parentless,
        // it has default properties; it should not
        // inherit its own empty quoteHub settings

        // assert
        quoteHub.Properties.Settings.Should().Be(0b00000000, "is has default settings, not inherited");
        renkoHub.Properties.Settings.Should().Be(0b00000010, "it has custom Renko properties");
        childHub.Properties.Settings.Should().Be(0b00000010, "it inherits Renko properties");
    }
}
