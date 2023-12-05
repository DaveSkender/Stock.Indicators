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

        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 50; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        // fetch initial results
        IEnumerable<UseResult> results = observer.Results;

        // emulate adding quotes to provider
        for (int i = 50; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        // final results
        List<UseResult> resultsList
            = results.ToList();

        // time-series, for comparison
        List<(DateTime, double)> seriesList = quotes
            .ToTuple(CandlePart.Close);

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            (DateTime date, double value) = seriesList[i];
            UseResult r = resultsList[i];

            Assert.AreEqual(date, r.Date);
            Assert.AreEqual(value, r.Value);
        }

        // confirm public interface
        Assert.AreEqual(observer.Cache.Count, observer.Results.Count());

        // confirm chain cache length (more tests in Chainor)
        Assert.AreEqual(observer.Cache.Count, observer.Chain.Count);

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

        // initialize observer
        Ema ema = provider
            .Use(CandlePart.HL2)
            .GetEma(11);

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        // stream results
        List<EmaResult> streamEma = ema
            .Results
            .ToList();

        // time-series, for comparison
        List<EmaResult> staticEma = quotes
            .Use(CandlePart.HL2)
            .GetEma(11)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            EmaResult s = staticEma[i];
            EmaResult r = streamEma[i];
            (DateTime date, double value) = ema.Chain[i];

            // compare series
            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Ema, r.Ema);

            // compare chain cache
            Assert.AreEqual(r.Date, date);
            Assert.AreEqual(r.Ema.Null2NaN(), value);
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
            // skip one
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
            UseResult r = observer.Cache[i];
            (DateTime Date, double Value) = observer.Chain[i];

            // compare quote to result cache
            Assert.AreEqual(q.Date, r.Date);
            Assert.AreEqual((double)q.Close, r.Value);

            // compare result to chain cache
            Assert.AreEqual(r.Date, Date);
            Assert.AreEqual(r.Value, Value);
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
                provider.Add(q);
            }
        });

        Assert.AreEqual(1, chainProvider.Results.Count());
        Assert.AreEqual(1, chainProvider.Chain.Count);
    }
}
