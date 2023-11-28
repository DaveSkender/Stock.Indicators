using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class ChainProviderTests : TestBase
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
            provider.CacheWithAnalysis(quotesList[i]);
        }

        // initialize Tuple-based observer
        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        // fetch initial results
        IEnumerable<BasicData> results = observer.Results;

        // emulate adding quotes to provider
        for (int i = 50; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        // final results
        List<BasicData> resultsList
            = results.ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            (DateTime sDate, double sValue) = seriesList[i];
            BasicData r = resultsList[i];

            Assert.AreEqual(sDate, r.Date);
            Assert.AreEqual(sValue, r.Value);
        }

        // confirm public interface
        Assert.AreEqual(observer.Cache.Count, observer.Results.Count());

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
        Ema<BasicData> ema = provider
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
            BasicData r = observer.Cache[i];

            Assert.AreEqual(q.Date, r.Date);
            Assert.AreEqual((double)q.Close, r.Value);
        }

        // close observations
        provider.EndTransmission();
    }

    [TestMethod]
    public void Overflow()
    {
        // initialize
        QuoteProvider<Quote> provider = new();

        Use<Quote> chainProvider = provider
            .Use(CandlePart.Close);

        // add too many duplicates
        Assert.ThrowsException<OverflowException>(() =>
        {
            Quote q = new() { Date = DateTime.Now };

            for (int i = 0; i <= 101; i++)
            {
                chainProvider.Add(q);
            }
        });

        Assert.AreEqual(1, chainProvider.Results.Count());
    }
}
