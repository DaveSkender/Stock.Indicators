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
        Ema observer = provider
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
    public void Usee()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // time-series, for comparison
        List<EmaResult> staticEma = quotes
            .Use(CandlePart.OC2)
            .GetEma(11)
            .ToList();

        // setup quote provider
        QuoteProvider provider = new();

        // initialize EMA observer
        List<EmaResult> streamEma = provider
            .Use(CandlePart.OC2)
            .GetEma(11)
            .ProtectedResults;

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            EmaResult t = staticEma[i];
            EmaResult s = streamEma[i];

            Assert.AreEqual(t.Date, s.Date);
            Assert.AreEqual(t.Ema, s.Ema);
        }
    }
}
