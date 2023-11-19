using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System.Collections.ObjectModel;
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

        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // initialize observer
        Sma observer = provider
            .GetSma(20);

        // fetch initial results
        IEnumerable<SmaResult> results
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
        List<SmaResult> resultsList
            = results.ToList();

        // time-series, for comparison
        List<SmaResult> seriesList = quotes
            .GetSma(20)
            .ToList();

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
    public void Manual()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // initialize
        Sma sma = new(14);

        // roll through history
        for (int i = 0; i < length; i++)
        {
            sma.Add(quotesList[i]);
        }

        // results
        List<SmaResult> resultList = sma.Results.ToList();

        // time-series, for comparison
        List<SmaResult> seriesList = quotes
            .GetSma(14)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            SmaResult s = seriesList[i];
            SmaResult r = resultList[i];

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Sma, r.Sma);
        }
    }

    [TestMethod]
    public void Increment()
    {
        double[] array = [1,2,3,4,5,6,7,8,9];

        double sma = Sma.Increment(array);

        Assert.AreEqual(5d, sma);
        Assert.AreEqual(array.Average(), sma);
    }

    [TestMethod]
    public void Usee()
    {
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

        // initialize EMA observer
        Sma observer = provider
            .Use(CandlePart.OC2)
            .GetSma(11);

        // emulate adding quotes to provider
        for (int i = 50; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        List<SmaResult> streamSma = observer
            .Results
            .ToList();

        // time-series, for comparison
        List<SmaResult> staticSma = quotes
            .Use(CandlePart.OC2)
            .GetSma(11)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            SmaResult s = staticSma[i];
            SmaResult r = streamSma[i];

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Sma, r.Sma);
        }
    }

    [TestMethod]
    public void Duplicates()
    {
        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // initialize SMA observer
        Sma observer = provider
            .GetSma(10);

        // add duplicate to cover warmup
        Quote quote = quotes.Last();

        for (int i = 0; i <= 20; i++)
        {
            provider.Add(quote);
        }

        observer.Unsubscribe();
        provider.EndTransmission();

        Assert.AreEqual(1, observer.Results.Count());
        Assert.AreEqual(1, observer.Tuples.Count());
    }
}
