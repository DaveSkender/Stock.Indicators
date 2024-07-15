namespace Tests.Common;

[TestClass]
public class Candles : TestBase
{
    [TestMethod]
    public void SortCandles()
    {
        IEnumerable<Quote> quotes = TestData.GetMismatch();

        // sort
        List<CandleProperties> candles
            = quotes.ToCandles().ToList();  // not sorted

        // proper quantities
        Assert.AreEqual(502, candles.Count);

        // sample values
        DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(firstDate, candles[0].Timestamp);

        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(lastDate, candles.LastOrDefault().Timestamp);

        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(spotDate, candles[50].Timestamp);
    }

    [TestMethod]
    public void CandleValues()
    {
        List<CandleProperties> candles
            = Quotes.ToCandles().ToList();  // not sorted

        // proper quantities
        Assert.AreEqual(502, candles.Count);

        // sample values
        CandleProperties r0 = candles[0];
        Assert.AreEqual(212.8m, r0.Close);
        Assert.AreEqual(1.83m, r0.Size);
        Assert.AreEqual(0.19m, r0.Body);
        Assert.AreEqual(0.55m, r0.UpperWick);
        Assert.AreEqual(1.09m, r0.LowerWick);
        Assert.AreEqual(0.10383, r0.BodyPct.Round(5));
        Assert.AreEqual(0.30055, r0.UpperWickPct.Round(5));
        Assert.AreEqual(0.59563, r0.LowerWickPct.Round(5));
        Assert.IsTrue(r0.IsBullish);
        Assert.IsFalse(r0.IsBearish);

        CandleProperties r351 = candles[351];
        Assert.AreEqual(1.24m, r351.Size);
        Assert.AreEqual(0m, r351.Body);
        Assert.AreEqual(0.69m, r351.UpperWick);
        Assert.AreEqual(0.55m, r351.LowerWick);
        Assert.AreEqual(0, r351.BodyPct.Round(5));
        Assert.AreEqual(0.55645, r351.UpperWickPct.Round(5));
        Assert.AreEqual(0.44355, r351.LowerWickPct.Round(5));
        Assert.IsFalse(r351.IsBullish);
        Assert.IsFalse(r351.IsBearish);

        CandleProperties r501 = candles[501];
        Assert.AreEqual(2.67m, r501.Size);
        Assert.AreEqual(0.36m, r501.Body);
        Assert.AreEqual(0.26m, r501.UpperWick);
        Assert.AreEqual(2.05m, r501.LowerWick);
        Assert.AreEqual(0.13483, r501.BodyPct.Round(5));
        Assert.AreEqual(0.09738, r501.UpperWickPct.Round(5));
        Assert.AreEqual(0.76779, r501.LowerWickPct.Round(5));
        Assert.IsTrue(r501.IsBullish);
        Assert.IsFalse(r501.IsBearish);
    }

    [TestMethod]
    public void ToCandles()
    {
        IEnumerable<CandleProperties> candles = Quotes.ToCandles();
        Assert.AreEqual(Quotes.Count(), candles.Count());
    }
}
