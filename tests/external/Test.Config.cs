using Internal.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace External.Other;

[TestClass]
public class ConfigTests
{
    [TestMethod]
    public void UseBadQuoteExceptionOff()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault(1);

        Indicator.UseConfig(
            new IndicatorConfig
            {
                UseBadQuotesException = false
            });

        // ADL
        IEnumerable<AdlResult> adl = quotes.GetAdl();
        Assert.AreEqual(1, adl.Count());

        // ADX
        IEnumerable<AdxResult> adx = quotes.GetAdx(5);
        Assert.AreEqual(1, adx.Count());

        // ATR
        IEnumerable<AtrResult> atr = quotes.GetAtr(5);
        Assert.AreEqual(1, atr.Count());

        // EMA
        IEnumerable<EmaResult> ema = quotes.GetEma(5);
        Assert.AreEqual(1, ema.Count());
    }

    [TestMethod]
    public void UseBadQuoteExceptionDefaultOn()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault(1);

        // ADL
        Assert.ThrowsException<BadQuotesException>(() => quotes.GetAdl());

        // ADX
        Assert.ThrowsException<BadQuotesException>(() => quotes.GetAdx(5));

        // ATR
        Assert.ThrowsException<BadQuotesException>(() => quotes.GetAtr(5));

        // EMA
        Assert.ThrowsException<BadQuotesException>(() => quotes.GetEma(5));
    }
}
