using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Rsi : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<RsiResult> results = quotes.GetRsi(14).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Rsi != null));

        // sample values
        RsiResult r1 = results[13];
        Assert.AreEqual(null, r1.Rsi);

        RsiResult r2 = results[14];
        Assert.AreEqual(62.0541, NullMath.Round(r2.Rsi, 4));

        RsiResult r3 = results[249];
        Assert.AreEqual(70.9368, NullMath.Round(r3.Rsi, 4));

        RsiResult r4 = results[501];
        Assert.AreEqual(42.0773, NullMath.Round(r4.Rsi, 4));
    }

    [TestMethod]
    public void SmallLookback()
    {
        int lookbackPeriods = 1;
        List<RsiResult> results = quotes.GetRsi(lookbackPeriods)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.Rsi != null));

        // sample values
        RsiResult r1 = results[28];
        Assert.AreEqual(100, r1.Rsi);

        RsiResult r2 = results[52];
        Assert.AreEqual(0, r2.Rsi);
    }

    [TestMethod]
    public void CryptoData()
    {
        IEnumerable<Quote> btc = TestData.GetBitcoin();
        IEnumerable<RsiResult> r = btc.GetRsi(1);
        Assert.AreEqual(1246, r.Count());
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<RsiResult> results = quotes
            .Use(CandlePart.Close)
            .GetRsi(14);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(488, results.Count(x => x.Rsi != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<RsiResult> r = tupleNanny.GetRsi(6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Rsi is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<RsiResult> results = quotes
            .GetSma(2)
            .GetRsi(14);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(487, results.Count(x => x.Rsi != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetRsi(14)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(479, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void NaN()
    {
        IEnumerable<RsiResult> r = TestData.GetBtcUsdNan()
            .GetRsi(14);

        Assert.AreEqual(0, r.Count(x => x.Rsi is double and double.NaN));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<RsiResult> r = badQuotes.GetRsi(20);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Rsi is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<RsiResult> r0 = noquotes.GetRsi();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<RsiResult> r1 = onequote.GetRsi();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<RsiResult> results = quotes.GetRsi(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (10 * 14), results.Count);

        RsiResult last = results.LastOrDefault();
        Assert.AreEqual(42.0773, NullMath.Round(last.Rsi, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetRsi(quotes, 0));
}
