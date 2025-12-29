namespace StreamHubs;

[TestClass]
public class WmaHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int LookbackPeriods = 14;

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        WmaHub observer = quoteHub.ToWmaHub(LookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<WmaResult> sut = observer.Results;

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

        IReadOnlyList<WmaResult> expectedOriginal = Quotes.ToWma(LookbackPeriods);
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<WmaResult> expectedRevised = RevisedQuotes.ToWma(LookbackPeriods);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        QuoteHub quoteHub = new();

        WmaHub observer = quoteHub
            .ToQuotePartHub(CandlePart.HL2)
            .ToWmaHub(LookbackPeriods);

        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        IReadOnlyList<WmaResult> streamList = observer.Results;
        IReadOnlyList<WmaResult> seriesList = quotesList
            .Use(CandlePart.HL2)
            .ToWma(LookbackPeriods);

        streamList.Should().HaveCount(length);
        streamList.IsExactly(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int emaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        QuoteHub quoteHub = new();

        EmaHub observer = quoteHub
            .ToWmaHub(LookbackPeriods)
            .ToEmaHub(emaPeriods);

        for (int i = 0; i < quotesCount; i++)
        {
            if (i == 80)
            {
                continue;
            }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        quoteHub.Insert(Quotes[80]);

        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<EmaResult> sut = observer.Results;
        IReadOnlyList<EmaResult> expected = RevisedQuotes
            .ToWma(LookbackPeriods)
            .ToEma(emaPeriods);

        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        WmaHub hub = new(new QuoteHub(), LookbackPeriods);
        hub.ToString().Should().Be($"WMA({LookbackPeriods})");
    }
}
