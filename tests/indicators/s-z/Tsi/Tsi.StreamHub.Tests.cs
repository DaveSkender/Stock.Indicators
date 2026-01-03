namespace StreamHubs;

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
        IReadOnlyList<TsiResult> sut = Quotes.ToTsiHub(25, 13, 7).Results;
        sut.IsBetween(static x => x.Tsi, -100, 100);
        sut.IsBetween(static x => x.Signal, -100, 100);
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        TsiHub observer = quoteHub.ToTsiHub(lookbackPeriods, smoothPeriods, signalPeriods);

        // fetch initial results (early)
        IReadOnlyList<TsiResult> sut = observer.Results;

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

        IReadOnlyList<TsiResult> expectedOriginal = Quotes.ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<TsiResult> expectedRevised = RevisedQuotes.ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
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
        streamList.IsExactly(seriesList);

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
        streamList.IsExactly(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
