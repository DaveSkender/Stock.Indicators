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

        // setup quote provider
        QuoteProvider provider = new();

        // initialize observer
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

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                provider.Add(q);
            }
        }

        // final results
        List<EmaResult> streamList
            = results.ToList();

        // time-series, for comparison
        List<EmaResult> seriesList = quotes
            .GetEma(20)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            EmaResult s = seriesList[i];
            EmaResult r = streamList[i];

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Ema, r.Ema);
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
        Ema ema = new(14);

        // roll through history
        for (int i = 0; i < length; i++)
        {
            ema.Increment(quotesList[i]);
        }

        // results
        List<EmaResult> resultList = ema.ProtectedResults;

        // time-series, for comparison
        List<EmaResult> seriesList = quotes
            .GetEma(14)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            EmaResult s = seriesList[i];
            EmaResult r = resultList[i];

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Ema, r.Ema);
        }
    }

    [TestMethod]
    public void Usee()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // time-series, for comparison
        List<EmaResult> seriesEma = quotes
            .Use(CandlePart.OC2)
            .GetEma(11)
            .ToList();

        // setup quote provider
        QuoteProvider provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 50; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize EMA observer
        List<EmaResult> streamEma = provider
            .Use(CandlePart.OC2)
            .GetEma(11)
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
            EmaResult s = seriesEma[i];
            EmaResult r = streamEma[i];

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Ema, r.Ema);
        }
    }

    [TestMethod]
    public void SelfIncrement()
    {
        // TODO: This test is entirely redundant to static Increment test, not needed

        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;
        int lookbackPeriods = 20;

        // time-series, for comparison
        List<EmaResult> seriesList = quotes
            .GetEma(lookbackPeriods)
            .ToList();

        // convert quote source to tuples
        System.Collections.ObjectModel.Collection<(DateTime, double)> tpList = quotes
            .Use(CandlePart.Close)
            .ToSortedCollection();

        // self-add increments
        List<EmaResult> resultList = new(length);
        double lastEma = double.NaN;

        double sum = 0d;
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            // prime initial quess
            if (i < lookbackPeriods - 1)
            {
                sum += value;

                resultList.Add(new(date));
            }

            // initial guess
            else if (i == lookbackPeriods - 1)
            {
                sum += value;
                lastEma = sum / lookbackPeriods;

                EmaResult r = new(date)
                {
                    Ema = lastEma
                };

                resultList.Add(r);
            }

            // increment
            else
            {
                EmaResult r = new(date)
                {
                    Ema = Ema.Increment(lookbackPeriods, lastEma, value)
                };

                resultList.Add(r);

                lastEma = r.Ema.Null2NaN();
            }
        }

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            EmaResult s = seriesList[i];
            EmaResult r = resultList[i];

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Ema, r.Ema);
        }
    }
}
