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
        QuoteProvider<Quote> provider = new();

        // initialize observer
        Sma<Quote> observer = provider
            .ToSma(20);

        // fetch initial results (early)
        IEnumerable<SmaResult> results
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

        // final results
        List<SmaResult> streamList
            = results.ToList();

        // time-series, for comparison
        List<SmaResult> seriesList = quotesList
            .GetSma(20)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            SmaResult s = seriesList[i];
            SmaResult r = streamList[i];

            Assert.AreEqual(s.Timestamp, r.Timestamp);
            Assert.AreEqual(s.Sma, r.Sma);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
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
        QuoteProvider<Quote> provider = new();

        // initialize observer
        Ema<SmaResult> observer = provider
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
        List<EmaResult> streamList
            = [.. observer.Results];

        // time-series, for comparison
        List<EmaResult> seriesList = quotesList
            .GetSma(smaPeriods)
            .GetEma(emaPeriods)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < quotesList.Count; i++)
        {
            EmaResult s = seriesList[i];
            EmaResult r = streamList[i];

            Assert.AreEqual(s.Timestamp, r.Timestamp);
            Assert.AreEqual(s.Ema, r.Ema);
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
        QuoteProvider<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 50; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        Sma<Reusable> observer = provider
            .Use(CandlePart.Oc2)
            .ToSma(11);

        // emulate quote stream
        for (int i = 50; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        List<SmaResult> streamSma =
            [.. observer.Results];

        // time-series, for comparison
        List<SmaResult> staticSma = Quotes
            .Use(CandlePart.Oc2)
            .GetSma(11)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            SmaResult s = staticSma[i];
            SmaResult r = streamSma[i];

            Assert.AreEqual(s.Timestamp, r.Timestamp);
            Assert.AreEqual(s.Sma, r.Sma);
        }
    }
}
