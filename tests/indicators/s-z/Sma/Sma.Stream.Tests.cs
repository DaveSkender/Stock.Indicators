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

        sma = Sma.Increment(tpList, tpList.Count - 1, 9);
        Assert.AreEqual(5d, sma);

        sma = Sma.Increment(tpList, tpList.Count - 1, 10);
        Assert.AreEqual(double.NaN, sma);
    }

    [TestMethod]
    public void Usee()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // time-series, for comparison
        List<SmaResult> staticSma = quotes
            .Use(CandlePart.OC2)
            .GetSma(11)
            .ToList();

        // setup quote provider
        QuoteProvider provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 50; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize EMA observer
        List<SmaResult> streamSma = provider
            .Use(CandlePart.OC2)
            .GetSma(11)
            .ProtectedResults;

        // emulate adding quotes to provider
        for (int i = 50; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            SmaResult t = staticSma[i];
            SmaResult s = streamSma[i];

            Assert.AreEqual(t.Date, s.Date);
            Assert.AreEqual(t.Sma, s.Sma);
        }
    }
}
