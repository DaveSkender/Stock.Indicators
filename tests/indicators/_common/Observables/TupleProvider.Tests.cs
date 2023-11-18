using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class TupleProviderTests : TestBase
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
        QuoteProvider<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 50; i++)
        {
            provider.CacheAndDeliverQuote(quotesList[i]);
        }

        // initialize Tuple-based observer
        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        // fetch initial results
        IEnumerable<(DateTime Date, double Value)> results
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 50; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.CacheAndDeliverQuote(q);
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

        // confirm public interface
        Assert.AreEqual(observer.ProtectedTuples.Count, observer.Results.Count());

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void Chainor()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // initialize EMA observer
        Ema ema = provider
            .Use(CandlePart.HL2)
            .GetEma(11);

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        // time-series, for comparison
        List<EmaResult> staticEma = quotes
            .Use(CandlePart.HL2)
            .GetEma(11)
            .ToList();

        // stream results
        List<EmaResult> streamEma = ema
            .Results
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            EmaResult e = staticEma[i];
            EmaResult r = streamEma[i];

            Assert.AreEqual(e.Date, r.Date);
            Assert.AreEqual(e.Ema, r.Ema);
        }
    }

    [TestMethod]
    public void LateArrival()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotes.Count();

        // add base quotes
        QuoteProvider<Quote> provider = new();

        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        // emulate incremental quotes
        for (int i = 0; i < length; i++)
        {
            if (i == 100)
            {
                continue;
            }

            Quote q = quotesList[i];
            provider.Add(q);
        }

        // add late
        provider.Add(quotesList[100]);

        // assert same as original
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            (DateTime date, double value) = observer.ProtectedTuples[i];

            Assert.AreEqual(q.Date, date);
            Assert.AreEqual((double)q.Close, value);
        }

        // close observations
        provider.EndTransmission();
    }

    [TestMethod]
    public void Overflow()
    {
        // initialize
        TupleProvider tp = new();

        // add too many duplicates
        Assert.ThrowsException<OverflowException>(() =>
        {
            DateTime d = DateTime.Now;

            for (int i = 0; i <= 101; i++)
            {
                tp.Add((d, 12345));
            }
        });

        Assert.AreEqual(1, tp.Tuples.Count());
    }
}
