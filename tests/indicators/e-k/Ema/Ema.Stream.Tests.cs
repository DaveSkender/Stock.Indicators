using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;
// TODO: standardize stream tests for EMA, SMA, ADL, ADX

[TestClass]
public class EmaStreamTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // initialize observer
        Ema<BasicData> observer = provider
            .GetEma(20);

        // fetch initial results
        IEnumerable<EmaResult> results
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                provider.Add(q);
            }
        }

        // final results
        List<EmaResult> streamList
            = results.ToList();

        // time-series, for comparison
        List<EmaResult> seriesList = quotes
            .GetEma(20)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            EmaResult s = seriesList[i];
            EmaResult r = streamList[i];

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Ema, r.Ema);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void Usee()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // time-series, for comparison
        List<EmaResult> seriesEma = quotes
            .Use(CandlePart.OC2)
            .GetEma(11)
            .ToList();

        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 50; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        List<EmaResult> streamEma = provider
            .Use(CandlePart.OC2)
            .GetEma(11)
            .Cache;

        // emulate adding quotes to provider
        for (int i = 50; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            EmaResult s = seriesEma[i];
            EmaResult r = streamEma[i];

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Ema, r.Ema);
        }
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
            .GetEma(emaPeriods)
            .GetSma(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // final results
        List<SmaResult> streamList
            = observer.Results.ToList();

        // time-series, for comparison
        List<SmaResult> seriesList = quotes
            .GetEma(emaPeriods)
            .GetSma(smaPeriods)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            SmaResult s = seriesList[i];
            SmaResult r = streamList[i];

            Assert.AreEqual(s.Date, r.Date);
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

        // initialize observer
        Ema<SmaResult> observer = provider
            .GetSma(smaPeriods)
            .GetEma(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // final results
        List<EmaResult> streamList
            = observer.Results.ToList();

        // time-series, for comparison
        List<EmaResult> seriesList = quotes
            .GetSma(smaPeriods)
            .GetEma(emaPeriods)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < quotesList.Count; i++)
        {
            EmaResult s = seriesList[i];
            EmaResult r = streamList[i];

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Ema, r.Ema);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void Duplicates()
    {
        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // initialize SMA observer
        Ema<BasicData> observer = provider
            .GetEma(10);

        // add duplicate to cover warmup
        Quote quote = quotes.Last();

        for (int i = 0; i <= 20; i++)
        {
            provider.Add(quote);
        }

        observer.Unsubscribe();
        provider.EndTransmission();

        Assert.AreEqual(1, observer.Results.Count());
    }

}
