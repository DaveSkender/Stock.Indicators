using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class EmaStreamTests : TestBase
{
    [TestMethod]
    public void StreamStandard()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // time-series, for comparison
        List<EmaResult> seriesList = quotesList
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
            = results
                .ToList();

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
    public void StreamInitBase()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        // time-series, for comparison
        List<EmaResult> series = quotesList
            .GetEma(20)
            .ToList();

        // setup quote provider
        QuoteProvider provider = new();

        // subscribe EMA as observer
        EmaObserver obsEma = new(provider, 20);

        List<Quote> baseQuotes = quotesList
            .Take(25)
            .ToList();

        obsEma.Initialize(baseQuotes);

        int[] dups = new int[] { 33, 67, 111, 250, 251 };

        for (int i = baseQuotes.Count; i < series.Count; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);

            // duplicate value + old value
            if (dups.Contains(i))
            {
                provider.Add(q);
            }
        }

        List<EmaResult> stream = obsEma
            .Results
            .ToList();

        // assert, should equal series
        for (int i = 0; i < series.Count; i++)
        {
            EmaResult t = series[i];
            EmaResult s = stream[i];

            Assert.AreEqual(t.Date, s.Date);
            Assert.AreEqual(t.Ema, s.Ema);
        }

        obsEma.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void StreamInitEmpty()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // time-series, for comparison
        List<EmaResult> series = quotesList
          .GetEma(20)
          .ToList();

        // setup quote provider
        QuoteProvider provider = new();

        // subscribe EMA as observer
        EmaObserver obsEma = new(provider, 20);

        int[] dups = new int[] { 3, 7, 11, 250, 251 };

        for (int i = 0; i < series.Count; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);

            // duplicate and values
            if (dups.Contains(i))
            {
                // duplicate
                provider.Add(q);
            }
        }

        List<EmaResult> stream = obsEma
             .Results
             .ToList();

        // assert, should equal series
        for (int i = 0; i < series.Count; i++)
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
        List<EmaResult> results = obsEma.Results.ToList();
        Assert.AreEqual(series[^6].Ema, results[^7].Ema);
        Assert.AreNotEqual(series[^1].Ema, results[^1].Ema);

        obsEma.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void Exceptions()
    {
        // null quote added
        QuoteProvider provider = new();
        EmaObserver obsEma = new(provider, 14);

        Assert.ThrowsException<InvalidQuotesException>(()
          => obsEma.OnNext(null));

        provider.EndTransmission();
    }
}
