namespace StreamHub;

[TestClass]
public class WmaHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int LookbackPeriods = 14;

    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        QuoteHub<Quote> quoteHub = new();

        for (int i = 0; i < LookbackPeriods; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        WmaHub<Quote> observer = quoteHub.ToWmaHub(LookbackPeriods);

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
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        QuoteHub<Quote> quoteHub = new();

        WmaHub<QuotePart> observer = quoteHub
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
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int emaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        QuoteHub<Quote> quoteHub = new();

        EmaHub<WmaResult> observer = quoteHub
            .ToWmaHub(LookbackPeriods)
            .ToEmaHub(emaPeriods);

        for (int i = 0; i < length; i++)
        {
            if (i == 90)
            {
                continue;
            }

            Quote quote = quotesList[i];
            quoteHub.Add(quote);

            if (i is > 180 and < 186)
            {
                quoteHub.Add(quote);
            }
        }

        quoteHub.Insert(quotesList[90]);

        quoteHub.Remove(quotesList[350]);
        quotesList.RemoveAt(350);

        IReadOnlyList<EmaResult> streamList = observer.Results;
        IReadOnlyList<EmaResult> seriesList = quotesList
            .ToWma(LookbackPeriods)
            .ToEma(emaPeriods);

        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        WmaHub<Quote> hub = new(new QuoteHub<Quote>(), LookbackPeriods);
        hub.ToString().Should().Be($"WMA({LookbackPeriods})");
    }
}
