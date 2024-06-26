namespace Tests.Indicators;

[TestClass]
public class EmaStreamTests : StreamTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // initialize observer
        Ema<Quote> observer = provider
            .ToEma(20);

        // fetch initial results (early)
        IEnumerable<EmaResult> results
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
        List<EmaResult> streamList
            = results.ToList();

        // time-series, for comparison
        List<EmaResult> seriesList = quotesList
            .GetEma(20)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
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
    public void Chainor()
    {
        int emaPeriods = 20;
        int smaPeriods = 10;

        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // initialize observer
        Sma<EmaResult> observer = provider
            .ToEma(emaPeriods)
            .ToSma(smaPeriods);

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
            = [.. observer.Results];

        // time-series, for comparison
        List<SmaResult> seriesList = quotesList
            .GetEma(emaPeriods)
            .GetSma(smaPeriods)
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
    public void Chainee()
    {
        int emaPeriods = 12;
        int smaPeriods = 8;

        List<Quote> quotesList = quotes
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
        Ema<SmaResult> observer = provider
            .ToSma(smaPeriods)
            .ToEma(emaPeriods);

        // emulate quote stream
        for (int i = 50; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // final results
        List<EmaResult> streamEma
            = [.. observer.Results];

        // time-series, for comparison
        List<EmaResult> staticEma = quotes
            .GetSma(smaPeriods)
            .GetEma(emaPeriods)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < quotesList.Count; i++)
        {
            EmaResult s = staticEma[i];
            EmaResult r = streamEma[i];

            Assert.AreEqual(s.Timestamp, r.Timestamp);
            Assert.AreEqual(s.Ema, r.Ema);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void Duplicates()
    {
        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // initialize observer
        Ema<Quote> observer = provider
            .ToEma<Quote>(10);

        // add duplicate to cover warmup
        Quote quote = quotes.Last();

        for (int i = 0; i <= 20; i++)
        {
            provider.Add(quote);
        }

        observer.Unsubscribe();
        provider.EndTransmission();

        Assert.AreEqual(1, observer.Results.Count);
        Assert.AreEqual(1, provider.Results.Count);
    }
}
