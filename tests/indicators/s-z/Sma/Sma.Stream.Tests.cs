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

            // Console.WriteLine($"{i,3}  {q.Date:s} ${q.Close,8:N2}  RESULTS: {observer.ProtectedResults.Count,3}  TUPLES {observer.TupleSupplier.ProtectedTuples.Count,3}  QUOTES: {provider.Quotes.Count(),3}");
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

            //(DateTime date, double value) = observer.ProtectedTuples[i];

            //Console.WriteLine($"{i,3} {s.Date:s} ${s.Sma,6:N2} {r.Date:s} ${r.Sma,6:N2} {date:s} ${value,6:N2}");

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Sma, r.Sma);
        }

        //Console.WriteLine($"RESULTS: {observer.ProtectedResults.Count,3} TUP: {observer.ProtectedTuples.Count}  SUP-TUP {observer.TupleSupplier.ProtectedTuples.Count,3}  QUOTES: {provider.Quotes.Count(),3}");
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
        // baseline for comparison
        Collection<(DateTime Date, double Value)> tpList =
        [
            new(DateTime.Parse("1/1/2000", EnglishCulture), 1d),
            new(DateTime.Parse("1/2/2000", EnglishCulture), 2d),
            new(DateTime.Parse("1/3/2000", EnglishCulture), 3d),
            new(DateTime.Parse("1/4/2000", EnglishCulture), 4d),
            new(DateTime.Parse("1/5/2000", EnglishCulture), 5d),
            new(DateTime.Parse("1/6/2000", EnglishCulture), 6d),
            new(DateTime.Parse("1/7/2000", EnglishCulture), 7d),
            new(DateTime.Parse("1/8/2000", EnglishCulture), 8d),
            new(DateTime.Parse("1/9/2000", EnglishCulture), 9d),
        ];

        //double sma = Sma.Increment(tpList);
        //Assert.AreEqual(5d, sma);
        Assert.Fail();
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

            (DateTime date, double value) = observer.ProtectedTuples[i];

            //Console.WriteLine($"{i,3} {s.Date:s} ${s.Sma,6:N2} {r.Date:s} ${r.Sma,6:N2} {date:s} ${value,6:N2}");

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Sma, r.Sma);
        }

        //Console.WriteLine($"RESULTS: {observer.ProtectedResults.Count,3} TUP: {observer.ProtectedTuples.Count}  SUP-TUP {observer.TupleSupplier.ProtectedTuples.Count,3}  QUOTES: {provider.Quotes.Count(),3}");
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
        Assert.AreEqual(1, observer.ResultTuples.Count());
    }
}
