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
        List<BaseQuote> o = quotes.GetBaseQuote(CandlePart.Open).ToList();
        List<BaseQuote> h = quotes.GetBaseQuote(CandlePart.High).ToList();
        List<BaseQuote> l = quotes.GetBaseQuote(CandlePart.Low).ToList();
        List<BaseQuote> c = quotes.GetBaseQuote(CandlePart.Close).ToList();
        List<BaseQuote> v = quotes.GetBaseQuote(CandlePart.Volume).ToList();
        List<BaseQuote> hl = quotes.GetBaseQuote(CandlePart.HL2).ToList();
        List<BaseQuote> hlc = quotes.GetBaseQuote(CandlePart.HLC3).ToList();
        List<BaseQuote> oc = quotes.GetBaseQuote(CandlePart.OC2).ToList();
        List<BaseQuote> ohl = quotes.GetBaseQuote(CandlePart.OHL3).ToList();
        List<BaseQuote> ohlc = quotes.GetBaseQuote(CandlePart.OHLC4).ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, c.Count);

        // samples
        BaseQuote ro = o[501];
        BaseQuote rh = h[501];
        BaseQuote rl = l[501];
        BaseQuote rc = c[501];
        BaseQuote rv = v[501];
        BaseQuote rhl = hl[501];
        BaseQuote rhlc = hlc[501];
        BaseQuote roc = oc[501];
        BaseQuote rohl = ohl[501];
        BaseQuote rohlc = ohlc[501];

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
        Assert.AreEqual(244.5633, Math.Round(rhlc.Value, 4));
        Assert.AreEqual(245.1, roc.Value);
        Assert.AreEqual(244.4433, Math.Round(rohl.Value, 4));
        Assert.AreEqual(244.6525, rohlc.Value);
    }
}
