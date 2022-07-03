using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class BaseQuoteTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        // compose basic data
        List<BasicData> o = quotes.GetBaseQuote(CandlePart.Open).ToList();
        List<BasicData> h = quotes.GetBaseQuote(CandlePart.High).ToList();
        List<BasicData> l = quotes.GetBaseQuote(CandlePart.Low).ToList();
        List<BasicData> c = quotes.GetBaseQuote(CandlePart.Close).ToList();
        List<BasicData> v = quotes.GetBaseQuote(CandlePart.Volume).ToList();
        List<BasicData> hl = quotes.GetBaseQuote(CandlePart.HL2).ToList();
        List<BasicData> hlc = quotes.GetBaseQuote(CandlePart.HLC3).ToList();
        List<BasicData> oc = quotes.GetBaseQuote(CandlePart.OC2).ToList();
        List<BasicData> ohl = quotes.GetBaseQuote(CandlePart.OHL3).ToList();
        List<BasicData> ohlc = quotes.GetBaseQuote(CandlePart.OHLC4).ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, c.Count);

        // samples
        BasicData ro = o[501];
        BasicData rh = h[501];
        BasicData rl = l[501];
        BasicData rc = c[501];
        BasicData rv = v[501];
        BasicData rhl = hl[501];
        BasicData rhlc = hlc[501];
        BasicData roc = oc[501];
        BasicData rohl = ohl[501];
        BasicData rohlc = ohlc[501];

        // proper last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, rc.Date);

        // last values should be correct
        Assert.AreEqual(244.92, ro.Value);
        Assert.AreEqual(245.54, rh.Value);
        Assert.AreEqual(242.87, rl.Value);
        Assert.AreEqual(245.28, rc.Value);
        Assert.AreEqual(147031456, rv.Value);
        Assert.AreEqual(244.205, rhl.Value);
        Assert.AreEqual(244.5633, NullMath.Round(rhlc.Value, 4));
        Assert.AreEqual(245.1, roc.Value);
        Assert.AreEqual(244.4433, NullMath.Round(rohl.Value, 4));
        Assert.AreEqual(244.6525, rohlc.Value);
    }

    [TestMethod]
    public void Use()
    {
        IEnumerable<(DateTime Date, double Value)> results = quotes
            .Use(CandlePart.Close);

        Assert.AreEqual(502, results.Count());
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetBaseQuote(CandlePart.Close)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }
}
