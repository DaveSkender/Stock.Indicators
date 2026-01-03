namespace StreamHubs;

[TestClass]
public class AroonHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<AroonResult> sut = Quotes.ToAroonHub(25).Results;
        sut.IsBetween(static x => x.AroonUp, 0, 100);
        sut.IsBetween(static x => x.AroonDown, 0, 100);
        sut.IsBetween(static x => x.Oscillator, -100, 100);
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider
        QuoteHub quoteHub = new();

        // prefill quotes to provider
        for (int i = 0; i < 30; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // initialize observer
        AroonHub aroonHub = quoteHub
            .ToAroonHub(25);

        // fetch initial results (early)
        IReadOnlyList<AroonResult> actuals
            = aroonHub.Results;

        // emulate adding quotes to provider
        for (int i = 30; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(Quotes[80]);

        // delete
        quoteHub.Remove(Quotes[removeAtIndex]);

        // time-series, for comparison
        IReadOnlyList<AroonResult> expected = RevisedQuotes.ToAroon(25);

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        aroonHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        AroonHub hub = new(new QuoteHub(), 25);
        hub.ToString().Should().Be("AROON(25)");
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        // Setup quote provider
        QuoteHub quoteHub = new();

        // Initialize observer - Aroon as provider feeding into EMA
        EmaHub emaHub = quoteHub
            .ToAroonHub(25)
            .ToEmaHub(12);

        // Emulate quote stream
        for (int i = 0; i < quotesCount; i++)
        {
            if (i == 80) { continue; }  // Skip for late arrival

            Quote q = Quotes[i];
            quoteHub.Add(q);

            if (i is > 100 and < 105) { quoteHub.Add(q); }  // Duplicate quotes
        }

        quoteHub.Insert(Quotes[80]);  // Late arrival
        quoteHub.Remove(Quotes[removeAtIndex]);  // Remove

        // Final results
        IReadOnlyList<EmaResult> sut = emaHub.Results;

        // Time-series, for comparison (revised)
        IReadOnlyList<EmaResult> expected = RevisedQuotes
            .ToAroon(25)
            .ToEma(12);

        // Assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        emaHub.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
