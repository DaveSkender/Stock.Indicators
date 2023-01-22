using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class EmaStreamTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // time-series, for comparison
        List<EmaResult> seriesList = quotes
            .GetEma(20)
            .ToList();

        // setup quote provider
        QuoteProvider provider = new();

        // initialize EMA observer
        EmaObserver observer = provider
            .GetEma(20);

        // fetch initial results
        IEnumerable<EmaResult> results
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        // final results
        List<EmaResult> resultsList
            = results.ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            EmaResult s = seriesList[i];
            EmaResult r = resultsList[i];

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Ema, r.Ema);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void LateArrival()
    {
        /********************************
         * 1. baseline batch of quotes
         * 2. duplicates of last date
         * 3. late arrival of old quotes
         ********************************/

        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // time-series, for comparison
        List<EmaResult> series = quotes
            .GetEma(20)
            .ToList();

        // setup quote provider
        QuoteProvider provider = new();

        // add "pre-existing" quotes
        List<Quote> baseQuotes = quotesList
            .Take(25)
            .ToList();

        provider.Add(baseQuotes);

        // subscribe as observer
        EmaObserver observer = provider.GetEma(20);

        // add quotes and last-date duplicates
        int[] dups = new int[] { 33, 67, 111, 250, 251 };

        for (int i = baseQuotes.Count; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);

            // duplicate value + old value
            if (dups.Contains(i))
            {
                provider.Add(q);
            }
        }

        List<EmaResult> stream = observer
            .Results
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            EmaResult t = series[i];
            EmaResult s = stream[i];

            Assert.AreEqual(t.Date, s.Date);
            Assert.AreEqual(t.Ema, s.Ema);
        }

        // throw in an extra old quote
        Quote og = quotesList[length - 5];

        // old modified quote
        Quote old = new()
        {
            Date = og.Date.AddHours(-52.25),
            High = og.High * 1.1m,
            Low = og.Low * 0.9m,
            Open = og.Low,
            Close = og.High * 0.95m,
            Volume = og.Volume * 1.9m
        };
        provider.Add(old);

        // should recalculate
        stream = observer
            .Results
            .ToList();

        Assert.AreEqual(series[^6].Ema, stream[^7].Ema);
        Assert.AreNotEqual(series[^1].Ema, stream[^1].Ema);

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
