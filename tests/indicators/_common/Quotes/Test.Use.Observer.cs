using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class UseStreamTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // time-series, for comparison
        List<(DateTime Date, double Value)> seriesList = quotes
            .ToTuple(CandlePart.Close);

        // setup quote provider
        QuoteProvider provider = new();

        // initialize EMA observer
        UseObserver observer = provider
            .Use(CandlePart.Close);

        // fetch initial results
        IEnumerable<(DateTime Date, double Value)> results
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        // final results
        List<(DateTime Date, double Value)> resultsList
            = results.ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            (DateTime sDate, double sValue) = seriesList[i];
            (DateTime rDate, double rValue) = resultsList[i];

            Assert.AreEqual(sDate, rDate);
            Assert.AreEqual(sValue, rValue);
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
        List<(DateTime Date, double Value)> series = quotes
            .ToTuple(CandlePart.OHL3);

        // setup quote provider
        QuoteProvider provider = new();

        // add "pre-existing" quotes
        List<Quote> baseQuotes = quotesList
            .Take(25)
            .ToList();

        provider.Add(baseQuotes);

        // subscribe as observer
        UseObserver observer = provider.Use(CandlePart.OHL3);

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

        List<(DateTime Date, double Value)> stream = observer
            .Results
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            (DateTime sDate, double sValue) = series[i];
            (DateTime tDate, double tValue) = stream[i];

            Assert.AreEqual(sDate, tDate);
            Assert.AreEqual(sValue, tValue);
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

        Assert.AreEqual(series[^6].Value, stream[^7].Value);
        Assert.AreEqual(series[^1].Value, stream[^1].Value);

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
