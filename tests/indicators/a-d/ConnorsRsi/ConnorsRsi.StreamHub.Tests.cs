namespace StreamHub;

[TestClass]
public class ConnorsRsiHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        ConnorsRsiHub observer = quoteHub
            .ToConnorsRsiHub(3, 2, 100);

        // fetch initial results (early)
        IReadOnlyList<ConnorsRsiResult> streamList
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
        IReadOnlyList<ConnorsRsiResult> seriesList = quotesList.ToConnorsRsi(3, 2, 100);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList, o => o.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        const int emaPeriods = 12;
        const int rsiPeriods = 3;
        const int streakPeriods = 2;
        const int rankPeriods = 100;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        ConnorsRsiHub observer = quoteHub
            .ToEmaHub(emaPeriods)
            .ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<ConnorsRsiResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<ConnorsRsiResult> seriesList
           = quotesList
            .ToEma(emaPeriods)
            .ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList, o => o.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int emaPeriods = 12;
        const int rsiPeriods = 3;
        const int streakPeriods = 2;
        const int rankPeriods = 100;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub observer = quoteHub
            .ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods)
            .ToEmaHub(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<EmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList
           = quotesList
            .ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods)
            .ToEma(emaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList, o => o.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub quoteHub = new();
        ConnorsRsiHub observer = quoteHub.ToConnorsRsiHub(3, 2, 100);

        observer.ToString().Should().Be("CRSI(3,2,100)");

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ResetBehavior()
    {
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with sample parameters
        ConnorsRsiHub observer = quoteHub.ToConnorsRsiHub(3, 2, 100);

        // Add ~50 quotes to populate state
        for (int i = 0; i < 50; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // assert observer.Results has 50 entries
        observer.Results.Should().HaveCount(50);

        // call observer.Reinitialize() - this resets the subscription and rebuilds from quoteHub
        observer.Reinitialize();

        // The observer should still have 50 results since it rebuilds from the quoteHub
        observer.Results.Should().HaveCount(50);

        // Now test with a completely fresh setup after unsubscribing
        observer.Unsubscribe();
        quoteHub.EndTransmission();

        // Create a new quoteHub with all quotes
        QuoteHub quoteHub2 = new();
        ConnorsRsiHub observer2 = quoteHub2.ToConnorsRsiHub(3, 2, 100);
        quoteHub2.Add(quotesList);

        // Verify results match fresh Series calculation
        IReadOnlyList<ConnorsRsiResult> seriesList = quotesList.ToConnorsRsi(3, 2, 100);
        observer2.Results.Should().HaveCount(quotesList.Count);
        observer2.Results.Should().BeEquivalentTo(seriesList, o => o.WithStrictOrdering());

        observer2.Unsubscribe();
        quoteHub2.EndTransmission();
    }
}
