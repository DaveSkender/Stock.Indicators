using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Sma : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<SmaResult> results = quotes.GetSma(20)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample values
        Assert.IsNull(results[18].Sma);
        Assert.AreEqual(214.5250, Math.Round(results[19].Sma.Value, 4));
        Assert.AreEqual(215.0310, Math.Round(results[24].Sma.Value, 4));
        Assert.AreEqual(234.9350, Math.Round(results[149].Sma.Value, 4));
        Assert.AreEqual(255.5500, Math.Round(results[249].Sma.Value, 4));
        Assert.AreEqual(251.8600, Math.Round(results[501].Sma.Value, 4));
    }

    [TestMethod]
    public void CandlePartOpen()
    {
        List<SmaResult> results = quotes
            .Use(CandlePart.Open)
            .GetSma(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample values
        Assert.IsNull(results[18].Sma);
        Assert.AreEqual(214.3795, Math.Round(results[19].Sma.Value, 4));
        Assert.AreEqual(214.9535, Math.Round(results[24].Sma.Value, 4));
        Assert.AreEqual(234.8280, Math.Round(results[149].Sma.Value, 4));
        Assert.AreEqual(255.6915, Math.Round(results[249].Sma.Value, 4));
        Assert.AreEqual(253.1725, Math.Round(results[501].Sma.Value, 4));
    }

    [TestMethod]
    public void CandlePartVolume()
    {
        List<SmaResult> results = quotes
            .Use(CandlePart.Volume)
            .GetSma(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample values
        SmaResult r24 = results[24];
        Assert.AreEqual(77293768.2, r24.Sma);

        SmaResult r290 = results[290];
        Assert.AreEqual(157958070.8, r290.Sma);

        SmaResult r501 = results[501];
        Assert.AreEqual(DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture), r501.Date);
        Assert.AreEqual(163695200, r501.Sma);
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<EmaResult> results = quotes
            .GetSma(10)
            .GetEma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(484, results.Count(x => x.Ema != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<SmaResult> r = tupleNanny.GetSma(6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Sma is double and double.NaN));
    }

    [TestMethod]
    public void NaN()
    {
        IEnumerable<SmaResult> r = TestData.GetBtcUsdNan()
            .GetSma(50);

        Assert.AreEqual(0, r.Count(x => x.Sma is double and double.NaN));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<SmaResult> r = Indicator.GetSma(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Sma is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<SmaResult> r0 = noquotes.GetSma(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<SmaResult> r1 = onequote.GetSma(5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<SmaResult> results = quotes.GetSma(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);
        Assert.AreEqual(251.8600, Math.Round(results.LastOrDefault().Sma.Value, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetSma(quotes, 0));
}
