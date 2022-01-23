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
        // update global configs
        Indicator.UseConfig(
            new IndicatorConfig
            {
                UseBadQuotesException = false
            });

        IEnumerable<Quote> quotes = TestData.GetDefault(1);
        IEnumerable<Quote> other = TestData.GetCompare(1);

        IEnumerable<AdlResult> adl = quotes.GetAdl(5);
        Assert.AreEqual(1, adl.Count());

        IEnumerable<AdxResult> adx = quotes.GetAdx(5);
        Assert.AreEqual(1, adx.Count());

        IEnumerable<AlligatorResult> alligator = quotes.GetAlligator();
        Assert.AreEqual(1, alligator.Count());

        IEnumerable<AlmaResult> alma = quotes.GetAlma(5);
        Assert.AreEqual(1, alma.Count());

        IEnumerable<AroonResult> aroon = quotes.GetAroon(5);
        Assert.AreEqual(1, aroon.Count());

        IEnumerable<AtrResult> atr = quotes.GetAtr(5);
        Assert.AreEqual(1, atr.Count());

        IEnumerable<AwesomeResult> awesome = quotes.GetAwesome(1, 2);
        Assert.AreEqual(1, awesome.Count());

        IEnumerable<BetaResult> beta = Indicator.GetBeta(quotes, other, 5);
        Assert.AreEqual(1, beta.Count());

        IEnumerable<BollingerBandsResult> bb = quotes.GetBollingerBands(5);
        Assert.AreEqual(1, bb.Count());

        IEnumerable<BopResult> bop = quotes.GetBop(5);
        Assert.AreEqual(1, bop.Count());

        IEnumerable<CciResult> cci = quotes.GetCci(5);
        Assert.AreEqual(1, cci.Count());

        IEnumerable<ChaikinOscResult> chosc = quotes.GetChaikinOsc(3, 5);
        Assert.AreEqual(1, chosc.Count());

        IEnumerable<ChandelierResult> chand = quotes.GetChandelier(5);
        Assert.AreEqual(1, chand.Count());

        IEnumerable<ChopResult> chop = quotes.GetChop(5);
        Assert.AreEqual(1, chop.Count());

        IEnumerable<CmfResult> cmf = quotes.GetCmf(5);
        Assert.AreEqual(1, cmf.Count());

        IEnumerable<ConnorsRsiResult> crsi = quotes.GetConnorsRsi();
        Assert.AreEqual(1, crsi.Count());

        IEnumerable<CorrResult> corr = Indicator.GetCorrelation(quotes, other, 5);
        Assert.AreEqual(1, corr.Count());

        IEnumerable<DonchianResult> don = quotes.GetDonchian(5);
        Assert.AreEqual(1, don.Count());

        IEnumerable<DemaResult> dema = quotes.GetDoubleEma(5);
        Assert.AreEqual(1, dema.Count());

        IEnumerable<DpoResult> dpo = quotes.GetDpo(5);
        Assert.AreEqual(1, dpo.Count());

        IEnumerable<ElderRayResult> eray = quotes.GetElderRay(5);
        Assert.AreEqual(1, eray.Count());

        IEnumerable<EmaResult> ema = quotes.GetEma(5);
        Assert.AreEqual(1, ema.Count());

        IEnumerable<EpmaResult> epma = quotes.GetEpma(5);
        Assert.AreEqual(1, epma.Count());

        IEnumerable<FcbResult> fcb = quotes.GetFcb(3);
        Assert.AreEqual(1, fcb.Count());

        IEnumerable<FisherTransformResult> ft = quotes.GetFisherTransform(5);
        Assert.AreEqual(1, ft.Count());

        IEnumerable<ForceIndexResult> fi = quotes.GetForceIndex(5);
        Assert.AreEqual(1, fi.Count());

        IEnumerable<FractalResult> fr = quotes.GetFractal();
        Assert.AreEqual(1, fr.Count());

        IEnumerable<GatorResult> gator = quotes.GetGator();
        Assert.AreEqual(1, gator.Count());

        IEnumerable<HeikinAshiResult> hka = quotes.GetHeikinAshi();
        Assert.AreEqual(1, hka.Count());

        IEnumerable<HmaResult> hma = quotes.GetHma(5);
        Assert.AreEqual(1, hma.Count());

        IEnumerable<HtlResult> ht = quotes.GetHtTrendline();
        Assert.AreEqual(1, ht.Count());

        IEnumerable<HurstResult> hurst = quotes.GetHurst(100);
        Assert.AreEqual(1, hurst.Count());

        IEnumerable<IchimokuResult> ichi = quotes.GetIchimoku();
        Assert.AreEqual(1, ichi.Count());

        IEnumerable<KamaResult> kama = quotes.GetKama();
        Assert.AreEqual(1, kama.Count());

        IEnumerable<KeltnerResult> kt = quotes.GetKeltner();
        Assert.AreEqual(1, kt.Count());

        IEnumerable<KvoResult> kvo = quotes.GetKvo();
        Assert.AreEqual(1, kvo.Count());

        IEnumerable<RsiResult> rsi = quotes.GetRsi(5);
        Assert.AreEqual(1, rsi.Count());

        IEnumerable<SlopeResult> slope = quotes.GetSlope(5);
        Assert.AreEqual(1, slope.Count());

        IEnumerable<SmaResult> sma = quotes.GetSma(5);
        Assert.AreEqual(1, sma.Count());

        IEnumerable<WmaResult> wma = quotes.GetWma(5);
        Assert.AreEqual(1, wma.Count());
    }
}
