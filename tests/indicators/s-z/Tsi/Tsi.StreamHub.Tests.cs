namespace StreamHub;

[TestClass]
public class TsiHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 25;
    private const int smoothPeriods = 13;
    private const int signalPeriods = 7;

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        TsiHub observer = quoteHub.ToTsiHub(lookbackPeriods, smoothPeriods, signalPeriods);

        // test string output
        observer.ToString().Should().Be("TSI(25,13,7)");

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<TsiResult> results = Quotes.ToTsiHub(25, 13, 7).Results;
        results.IsBetween(x => x.Tsi, -100, 100);
        results.IsBetween(x => x.Signal, -100, 100);
    }

    [TestMethod]
    public void OldBoundaryTest()
    {
        QuoteHub quoteHub = new();
        TsiHub observer = quoteHub.ToTsiHub(lookbackPeriods, smoothPeriods, signalPeriods);

        foreach (Quote quote in Quotes)
        {
            quoteHub.Add(quote);
        }

        observer.Results.IsBetween(x => x.Tsi, -100, 100);
        observer.Results.IsBetween(x => x.Signal, -100, 100);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
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
        TsiHub observer = quoteHub.ToTsiHub(lookbackPeriods, smoothPeriods, signalPeriods);

        // fetch initial results (early)
        IReadOnlyList<TsiResult> streamList = observer.Results;

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
        quoteHub.Remove(quotesList[removeAtIndex]);
        quotesList.RemoveAt(removeAtIndex);

        // time-series, for comparison
        IReadOnlyList<TsiResult> seriesList = quotesList.ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int emaPeriods = 12;

        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        TsiHub observer = quoteHub
            .ToEmaHub(emaPeriods)
            .ToTsiHub(lookbackPeriods, smoothPeriods, signalPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<TsiResult> streamList = observer.Results;

        // time-series, for comparison
        IReadOnlyList<TsiResult> seriesList = quotesList
            .ToEma(emaPeriods)
            .ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToTsiHub(lookbackPeriods, smoothPeriods, signalPeriods)
            .ToSmaHub(smaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < length; i++)
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
        quoteHub.Remove(quotesList[removeAtIndex]);
        quotesList.RemoveAt(removeAtIndex);

        // final results
        IReadOnlyList<SmaResult> streamList = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList = quotesList
            .ToTsi(lookbackPeriods, smoothPeriods, signalPeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
