using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class HeikinAshiTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<HeikinAshiResult> results = quotes
            .GetHeikinAshi()
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);

        // sample value
        HeikinAshiResult r = results[501];
        Assert.AreEqual(241.3018m, r.Open.Round(4));
        Assert.AreEqual(245.54m, r.High.Round(4));
        Assert.AreEqual(241.3018m, r.Low.Round(4));
        Assert.AreEqual(244.6525m, r.Close.Round(4));
        Assert.AreEqual(147031456m, r.Volume);
    }

    [TestMethod]
    public void UseAsQuotes()
    {
        IEnumerable<HeikinAshiResult> haQuotes = quotes.GetHeikinAshi();
        IEnumerable<SmaResult> haSma = haQuotes.GetSma(5);
        Assert.AreEqual(498, haSma.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void ToQuotes()
    {
        List<HeikinAshiResult> results = quotes
            .GetHeikinAshi()
            .ToList();

        List<Quote> haQuotes = results
            .ToQuotes()
            .ToList();

        for (int i = 0; i < results.Count; i++)
        {
            HeikinAshiResult r = results[i];
            Quote q = haQuotes[i];

            Assert.AreEqual(r.Date, q.Date);
            Assert.AreEqual(r.Open, q.Open);
            Assert.AreEqual(r.High, q.High);
            Assert.AreEqual(r.Low, q.Low);
            Assert.AreEqual(r.Close, q.Close);
            Assert.AreEqual(r.Volume, q.Volume);
        }
    }

    [TestMethod]
    public void BadData()
    {
        List<HeikinAshiResult> r = badQuotes
            .GetHeikinAshi()
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<HeikinAshiResult> r0 = noquotes
            .GetHeikinAshi()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<HeikinAshiResult> r1 = onequote
            .GetHeikinAshi()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }
}
