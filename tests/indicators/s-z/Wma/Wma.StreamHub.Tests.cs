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

        QuoteHub<Quote> provider = new();

        for (int i = 0; i < LookbackPeriods; i++)
        {
            provider.Add(quotesList[i]);
        }

        WmaHub<Quote> observer = provider.ToWma(LookbackPeriods);

        for (int i = LookbackPeriods; i < length; i++)
        {
            if (i == 80)
            {
                continue;
            }

            Quote quote = quotesList[i];
            provider.Add(quote);

            if (i is > 110 and < 115)
            {
                provider.Add(quote);
            }
        }

        provider.Insert(quotesList[80]);

        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        IReadOnlyList<WmaResult> streamList = observer.Results;
        IReadOnlyList<WmaResult> seriesList = quotesList.ToWma(LookbackPeriods);

        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        WmaHub<QuotePart> observer = provider
            .ToQuotePart(CandlePart.HL2)
            .ToWma(LookbackPeriods);

        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        IReadOnlyList<WmaResult> streamList = observer.Results;
        IReadOnlyList<WmaResult> seriesList = quotesList
            .Use(CandlePart.HL2)
            .ToWma(LookbackPeriods);

        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int emaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        EmaHub<WmaResult> observer = provider
            .ToWma(LookbackPeriods)
            .ToEma(emaPeriods);

        for (int i = 0; i < length; i++)
        {
            if (i == 90)
            {
                continue;
            }

            Quote quote = quotesList[i];
            provider.Add(quote);

            if (i is > 180 and < 186)
            {
                provider.Add(quote);
            }
        }

        provider.Insert(quotesList[90]);

        provider.Remove(quotesList[350]);
        quotesList.RemoveAt(350);

        IReadOnlyList<EmaResult> streamList = observer.Results;
        IReadOnlyList<EmaResult> seriesList = quotesList
            .ToWma(LookbackPeriods)
            .ToEma(emaPeriods);

        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        WmaHub<Quote> hub = new(new QuoteHub<Quote>(), LookbackPeriods);
        hub.ToString().Should().Be($"WMA({LookbackPeriods})");
    }
}
