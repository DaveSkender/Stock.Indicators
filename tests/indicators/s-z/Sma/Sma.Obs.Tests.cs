using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class SmaStreamTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // time-series, for comparison
        List<SmaResult> seriesList = quotes
            .GetSma(20)
            .ToList();

        // setup quote provider
        QuoteProvider provider = new();

        // initialize EMA observer
        SmaObserver observer = provider
            .GetSma(20);

        // fetch initial results
        IEnumerable<SmaResult> results
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        // final results
        List<SmaResult> resultsList
            = results.ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            SmaResult s = seriesList[i];
            SmaResult r = resultsList[i];

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Sma, r.Sma);
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
        List<SmaResult> series = quotes
            .GetSma(20)
            .ToList();

        // setup quote provider
        QuoteProvider provider = new();

        // add "pre-existing" quotes
        List<Quote> baseQuotes = quotesList
            .Take(25)
            .ToList();

        provider.Add(baseQuotes);

        // subscribe as observer
        SmaObserver observer = provider.GetSma(20);

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

        List<SmaResult> stream = observer
            .Results
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            SmaResult t = series[i];
            SmaResult s = stream[i];

            Assert.AreEqual(t.Date, s.Date);
            Assert.AreEqual(t.Sma, s.Sma);
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

        Assert.AreEqual(series[^6].Sma, stream[^7].Sma);
        Assert.AreNotEqual(series[^1].Sma, stream[^1].Sma);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void Increment()
    {
        // baseline for comparison
        List<(DateTime Date, double Value)> tpList = new()
        {
            new (DateTime.Parse("1/1/2000", EnglishCulture), 1d),
            new (DateTime.Parse("1/2/2000", EnglishCulture), 2d),
            new (DateTime.Parse("1/3/2000", EnglishCulture), 3d),
            new (DateTime.Parse("1/4/2000", EnglishCulture), 4d),
            new (DateTime.Parse("1/5/2000", EnglishCulture), 5d),
            new (DateTime.Parse("1/6/2000", EnglishCulture), 6d),
            new (DateTime.Parse("1/7/2000", EnglishCulture), 7d),
            new (DateTime.Parse("1/8/2000", EnglishCulture), 8d),
            new (DateTime.Parse("1/9/2000", EnglishCulture), 9d),
        };

        double sma;

        sma = SmaObserver.Increment(tpList, tpList.Count - 1, 9);
        Assert.AreEqual(5d, sma);

        sma = SmaObserver.Increment(tpList, tpList.Count - 1, 10);
        Assert.AreEqual(double.NaN, sma);
    }
}
