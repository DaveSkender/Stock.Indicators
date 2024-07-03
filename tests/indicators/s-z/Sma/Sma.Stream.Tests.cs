namespace Tests.Indicators.Stream;

[TestClass]
public class SmaTests : StreamTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        SmaHub<Quote> observer = provider
            .ToSma(20);

        // fetch initial results (early)
        IReadOnlyList<SmaResult> streamList
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
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
        provider.Delete(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        List<SmaResult> seriesList
           = quotesList
            .GetSma(20)
            .ToList();

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
    public void ChainObserver()
    {
        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 50; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        SmaHub<Reusable> observer = provider
            .Use(CandlePart.OC2)
            .ToSma(11);

        // emulate quote stream
        for (int i = 50; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        IReadOnlyList<SmaResult> streamList =
            observer.Results;

        // time-series, for comparison
        List<SmaResult> staticList
           = quotesList
            .Use(CandlePart.OC2)
            .GetSma(11)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            SmaResult s = staticList[i];
            SmaResult r = streamList[i];

            r.Timestamp.Should().Be(q.Timestamp);
            r.Timestamp.Should().Be(s.Timestamp);
            r.Sma.Should().Be(s.Sma);
            r.Should().Be(s);
        }
    }

    [TestMethod]
    public void ChainProvider()
    {
        int emaPeriods = 12;
        int smaPeriods = 8;

        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        EmaHub<SmaResult> observer
           = provider
            .ToSma(smaPeriods)
            .ToEma(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // delete
        provider.Delete(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<EmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        List<EmaResult> seriesList
           = quotesList
            .GetSma(smaPeriods)
            .GetEma(emaPeriods)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length - 1; i++)
        {
            Quote q = quotesList[i];
            EmaResult s = seriesList[i];
            EmaResult r = streamList[i];

            r.Timestamp.Should().Be(q.Timestamp);
            r.Timestamp.Should().Be(s.Timestamp);
            r.Ema.Should().Be(s.Ema);
            r.Should().Be(s);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
