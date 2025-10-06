namespace StreamHub;

[TestClass]
public class TrixHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 20; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        TrixHub<Quote> observer = provider
            .ToTrix(14);

        // fetch initial results (early)
        IReadOnlyList<TrixResult> streamList
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            provider.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                provider.Add(q);
            }
        }

        // late arrival
        provider.Insert(quotesList[80]);

        // delete
        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<TrixResult> seriesList = quotesList.ToTrix(14);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        int trixPeriods = 14;
        int smaPeriods = 8;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        TrixHub<SmaResult> observer = provider
            .ToSma(smaPeriods)
            .ToTrix(trixPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<TrixResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<TrixResult> seriesList
           = quotesList
            .ToSma(smaPeriods)
            .ToTrix(trixPeriods);

        // assert
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        int trixPeriods = 14;
        int emaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize chain with TRIX as input to EMA
        EmaHub<TrixResult> emaOfTrix = provider
            .ToTrix(trixPeriods)
            .ToEma(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<EmaResult> streamList = emaOfTrix.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList = quotesList
            .ToTrix(trixPeriods)
            .ToEma(emaPeriods);

        // assert
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        emaOfTrix.Unsubscribe();
        provider.EndTransmission();
    }

    public override void CustomToString()
    {
        TrixHub<Quote> hub = new(new QuoteHub<Quote>(), 14);
        hub.ToString().Should().Be("TRIX(14)");
    }
}
