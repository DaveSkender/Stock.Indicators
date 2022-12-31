using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class EmaStreamTests : TestBase
{
    [TestMethod]
    public void StreamInitBase()
    {
        List<Quote> quotesList = quotes
            .OrderBy(x => x.Date)
            .ToList();

        // time-series, for comparison
        List<EmaResult> series = quotesList
            .GetEma(20)
            .ToList();

        // setup quote provider
        QuoteProvider provider = new();

        // subscribe EMA as observer
        EmaObs obsEma = new(provider, 20);

        IEnumerable<Quote> baseQuotes = quotesList.Take(25);
        obsEma.Initialize(baseQuotes);

        int[] dups = new int[] { 33, 67, 111, 250, 251 };

        for (int i = 25; i < series.Count; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);

            // duplicate value
            if (dups.Contains(i))
            {
                provider.Add(q);
            }
        }

        List<EmaResult> stream = obsEma.Results.ToList();

        // assert, should equal series
        for (int i = 0; i < series.Count; i++)
        {
            EmaResult t = series[i];
            EmaResult s = stream[i];

            Assert.AreEqual(t.Date, s.Date);
            Assert.AreEqual(t.Ema, s.Ema);
        }

        obsEma.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void StreamInitEmpty()
    {
        List<Quote> quotesList = quotes
            .OrderBy(x => x.Date)
            .ToList();

        // time-series, for comparison
        List<EmaResult> series = quotesList
          .GetEma(20)
          .ToList();

        // setup quote provider
        QuoteProvider provider = new();

        // subscribe EMA as observer
        EmaObs obsEma = new(provider, 20);

        int[] dups = new int[] { 3, 7, 11, 250, 251 };

        for (int i = 0; i < series.Count; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);

            // duplicate value
            if (dups.Contains(i))
            {
                provider.Add(q);
            }
        }

        List<EmaResult> stream = obsEma
          .Results
          .ToList();

        // assert, should equal series
        for (int i = 0; i < series.Count; i++)
        {
            EmaResult t = series[i];
            EmaResult s = stream[i];

            Assert.AreEqual(t.Date, s.Date);
            Assert.AreEqual(t.Ema, s.Ema);
        }

        obsEma.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void Exceptions()
    {
        // null quote added
        QuoteProvider provider = new();
        EmaObs obsEma = new(provider, 14);

        Assert.ThrowsException<InvalidQuotesException>(()
          => obsEma.OnNext(null));

        provider.EndTransmission();
    }
}
