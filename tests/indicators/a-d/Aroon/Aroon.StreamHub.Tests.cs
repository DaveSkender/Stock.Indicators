namespace StreamHub;

[TestClass]
public class AroonHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        AroonResult[] results = [.. Quotes.ToAroonHub(25).Results];
        TestAsserts.AlwaysBounded(results, x => x.AroonUp, 0, 100);
        TestAsserts.AlwaysBounded(results, x => x.AroonDown, 0, 100);
        TestAsserts.AlwaysBounded(results, x => x.Oscillator, -100, 100);
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub quoteHub = new();

        // prefill quotes to provider
        for (int i = 0; i < 30; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        AroonHub aroonHub = quoteHub
            .ToAroonHub(25);

        // fetch initial results (early)
        IReadOnlyList<AroonResult> streamList
            = aroonHub.Results;

        // emulate adding quotes to provider
        for (int i = 30; i < length; i++)
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
        IReadOnlyList<AroonResult> seriesList = quotesList.ToAroon(25);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        aroonHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        AroonHub aroonHub = quoteHub.ToAroonHub(25);

        aroonHub.ToString().Should().Be("AROON(25)");

        aroonHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // Setup quote provider
        QuoteHub quoteHub = new();

        // Initialize observer - Aroon as provider feeding into EMA
        EmaHub emaHub = quoteHub
            .ToAroonHub(25)
            .ToEmaHub(12);

        // Emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // Final results
        IReadOnlyList<EmaResult> streamList = emaHub.Results;

        // Time-series, for comparison
        IReadOnlyList<EmaResult> seriesList = quotesList
            .ToAroon(25)
            .ToEma(12);

        // Assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        emaHub.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
