namespace StreamHub;

[TestClass]
public class UseTests : StreamHubTestBase, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        CandlePart candlePart = CandlePart.HLC3;

        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 20; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        QuotePartHub<Quote> observer = provider
            .ToQuotePart(candlePart);

        // fetch initial results (early)
        IReadOnlyList<QuotePart> streamList
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
        provider.Add(quotesList[80]);

        // delete
        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<QuotePart> seriesList
           = quotesList
            .Use(candlePart);

        // assert, should equal series
        for (int i = 0; i < length - 1; i++)
        {
            Quote q = quotesList[i];
            QuotePart s = seriesList[i];
            QuotePart r = streamList[i];

            r.Timestamp.Should().Be(q.Timestamp);
            r.Timestamp.Should().Be(s.Timestamp);
            r.Value.Should().Be(s.Value);
            r.Should().Be(s);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        int smaPeriods = 8;
        CandlePart candlePart = CandlePart.OHLC4;

        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        SmaHub<QuotePart> observer
           = provider
            .ToQuotePart(candlePart)
            .ToSma(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // delete
        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<SmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList
           = quotesList
            .Use(candlePart)
            .GetSma(smaPeriods);

        // assert, should equal series
        for (int i = 0; i < length - 1; i++)
        {
            Quote q = quotesList[i];
            SmaResult s = seriesList[i];
            SmaResult r = streamList[i];

            r.Timestamp.Should().Be(q.Timestamp);
            r.Timestamp.Should().Be(s.Timestamp);
            r.Sma.Should().Be(s.Sma);
            r.Should().Be(s);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuotePartHub<Quote> hub = new(new QuoteHub<Quote>(), CandlePart.Close);
        hub.ToString().Should().Be("QUOTE-PART(CLOSE)");
    }
}
