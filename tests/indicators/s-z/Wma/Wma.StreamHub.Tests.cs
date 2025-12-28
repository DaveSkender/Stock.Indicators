namespace StreamHubs;

[TestClass]
public class WmaHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int LookbackPeriods = 14;

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        QuoteHub quoteHub = new();

        for (int i = 0; i < LookbackPeriods; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        WmaHub observer = quoteHub.ToWmaHub(LookbackPeriods);

        for (int i = LookbackPeriods; i < length; i++)
        {
            if (i == 80)
            {
                continue;
            }

            Quote quote = quotesList[i];
            quoteHub.Add(quote);

            if (i is > 110 and < 115)
            {
                quoteHub.Add(quote);
            }
        }

        quoteHub.Insert(quotesList[80]);

        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        IReadOnlyList<WmaResult> streamList = observer.Results;
        IReadOnlyList<WmaResult> seriesList = quotesList.ToWma(LookbackPeriods);

        streamList.Should().HaveCount(length - 1);
        streamList.IsExactly(seriesList);

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
