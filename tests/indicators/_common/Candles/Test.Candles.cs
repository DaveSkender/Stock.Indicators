using Internal.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

[TestClass]
public class Candles : TestBase
{
    [TestMethod]
    public void SortCandles()
    {
        IEnumerable<Quote> quotes = TestData.GetMismatch();

        // sort
        List<CandleResult> candles = quotes.ToCandleResults();

        // proper quantities
        Assert.AreEqual(502, candles.Count);

        // sample values
        DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(firstDate, candles[0].Date);

        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, candles.LastOrDefault().Date);

        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(spotDate, candles[50].Date);
    }

    [TestMethod]
    public void CandleValues()
    {
        List<CandleResult> candles = quotes.ToCandleResults();

        // proper quantities
        Assert.AreEqual(502, candles.Count);

        // sample values
        CandleResult r0 = candles[0];
        Assert.AreEqual(212.8m, r0.Candle.Close);
        Assert.AreEqual(1.83m, r0.Candle.Size);
        Assert.AreEqual(0.19m, r0.Candle.Body);
        Assert.AreEqual(0.55m, r0.Candle.UpperWick);
        Assert.AreEqual(1.09m, r0.Candle.LowerWick);
        Assert.AreEqual(0.10383, r0.Candle.BodyPct.Round(5));
        Assert.AreEqual(0.30055, r0.Candle.UpperWickPct.Round(5));
        Assert.AreEqual(0.59563, r0.Candle.LowerWickPct.Round(5));
        Assert.IsTrue(r0.Candle.IsBullish);
        Assert.IsFalse(r0.Candle.IsBearish);

        CandleResult r351 = candles[351];
        Assert.AreEqual(1.24m, r351.Candle.Size);
        Assert.AreEqual(0m, r351.Candle.Body);
        Assert.AreEqual(0.69m, r351.Candle.UpperWick);
        Assert.AreEqual(0.55m, r351.Candle.LowerWick);
        Assert.AreEqual(0, r351.Candle.BodyPct.Round(5));
        Assert.AreEqual(0.55645, r351.Candle.UpperWickPct.Round(5));
        Assert.AreEqual(0.44355, r351.Candle.LowerWickPct.Round(5));
        Assert.IsFalse(r351.Candle.IsBullish);
        Assert.IsFalse(r351.Candle.IsBearish);

        CandleResult r501 = candles[501];
        Assert.AreEqual(2.67m, r501.Candle.Size);
        Assert.AreEqual(0.36m, r501.Candle.Body);
        Assert.AreEqual(0.26m, r501.Candle.UpperWick);
        Assert.AreEqual(2.05m, r501.Candle.LowerWick);
        Assert.AreEqual(0.13483, r501.Candle.BodyPct.Round(5));
        Assert.AreEqual(0.09738, r501.Candle.UpperWickPct.Round(5));
        Assert.AreEqual(0.76779, r501.Candle.LowerWickPct.Round(5));
        Assert.IsTrue(r501.Candle.IsBullish);
        Assert.IsFalse(r501.Candle.IsBearish);
    }

    [TestMethod]
    public void ToCandles()
    {
        IEnumerable<CandleProperties> candles = quotes.ToCandles();
        Assert.AreEqual(quotes.Count(), candles.Count());
    }
}
