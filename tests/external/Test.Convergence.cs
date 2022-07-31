using Internal.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace External.Other;

[TestClass]
public class Convergence : TestBase
{
    private static readonly int[] QuotesQuantities =
    new int[] { 5, 14, 28, 40, 50, 75, 100, 110, 120, 130, 140, 150, 160, 175, 200, 250, 350, 500, 600, 700, 800, 900, 1000 };

    [TestMethod]
    public void Adx()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<AdxResult> r = quotes.GetAdx(14);

            AdxResult l = r.LastOrDefault();
            Console.WriteLine(
                "ADX(14) on {0:d} with {1,4} historical quotes: {2:N8}",
                l.Date, quotes.Count(), l.Adx);
        }
    }

    [TestMethod]
    public void Atr()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<AtrResult> r = quotes.GetAtr(14);

            AtrResult l = r.LastOrDefault();
            Console.WriteLine(
                "ATR(14) on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Atr);
        }
    }

    [TestMethod]
    public void ChaikinOsc()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<ChaikinOscResult> r = quotes.GetChaikinOsc();

            ChaikinOscResult l = r.LastOrDefault();
            Console.WriteLine(
                "CHAIKIN OSC on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Oscillator);
        }
    }

    [TestMethod]
    public void ConnorsRsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<ConnorsRsiResult> r = quotes.GetConnorsRsi(3, 2, 10);

            ConnorsRsiResult l = r.LastOrDefault();
            Console.WriteLine(
                "CRSI on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.ConnorsRsi);
        }
    }

    [TestMethod]
    public void Dema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<DemaResult> r = quotes.GetDema(15);

            DemaResult l = r.LastOrDefault();
            Console.WriteLine(
                "DEMA(15) on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Dema);
        }
    }

    [TestMethod]
    public void Ema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<EmaResult> r = quotes.GetEma(15);

            EmaResult l = r.LastOrDefault();
            Console.WriteLine(
                "EMA(15) on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Ema);
        }
    }

    [TestMethod]
    public void FisherTransform()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<FisherTransformResult> r = quotes.GetFisherTransform(10);

            FisherTransformResult l = r.LastOrDefault();
            Console.WriteLine(
                "FT(10) on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Fisher);
        }
    }

    [TestMethod]
    public void HtTrendline()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<HtlResult> r = quotes.GetHtTrendline();

            HtlResult l = r.LastOrDefault();
            Console.WriteLine(
                "HTL on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Trendline);
        }
    }

    [TestMethod]
    public void Kama()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<KamaResult> r = quotes.GetKama(10);

            KamaResult l = r.LastOrDefault();
            Console.WriteLine(
                "KAMA(10) on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Kama);
        }
    }

    [TestMethod]
    public void Keltner()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<KeltnerResult> r = quotes.GetKeltner(100);

            KeltnerResult l = r.LastOrDefault();
            Console.WriteLine(
                "KC-UP on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.UpperBand);
        }
    }

    [TestMethod]
    public void Macd()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(15 + qty);
            IEnumerable<MacdResult> r = quotes.GetMacd();

            MacdResult l = r.LastOrDefault();
            Console.WriteLine(
                "MACD on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Macd);
        }
    }

    [TestMethod]
    public void Mama()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<MamaResult> r = quotes.GetMama();

            MamaResult l = r.LastOrDefault();
            Console.WriteLine(
                "MAMA on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Mama);
        }
    }

    [TestMethod]
    public void Pmo()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<PmoResult> r = quotes.GetPmo();

            PmoResult l = r.LastOrDefault();
            Console.WriteLine(
                "PMO on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Pmo);
        }
    }

    [TestMethod]
    public void Pvo()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<PvoResult> r = quotes.GetPvo();

            PvoResult l = r.LastOrDefault();
            Console.WriteLine(
                "PVO on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Pvo);
        }
    }

    [TestMethod]
    public void Rsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<RsiResult> r = quotes.GetRsi(14);

            RsiResult l = r.LastOrDefault();
            Console.WriteLine(
                "RSI(14) on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Rsi);
        }
    }

    [TestMethod]
    public void Smi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetDefault(qty);
            IEnumerable<SmiResult> r = quotes.GetSmi(14, 20, 5, 3);

            SmiResult l = r.LastOrDefault();
            Console.WriteLine(
                "SMI on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Smi);
        }
    }

    [TestMethod]
    public void Smma()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<SmmaResult> r = quotes.GetSmma(15);

            SmmaResult l = r.LastOrDefault();
            Console.WriteLine(
                "SMMA(15) on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Smma);
        }
    }

    [TestMethod]
    public void StarcBands()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<StarcBandsResult> r = quotes.GetStarcBands(20);

            StarcBandsResult l = r.LastOrDefault();
            Console.WriteLine(
                "STARC UPPER on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.UpperBand);
        }
    }

    [TestMethod]
    public void StochRsi()
    {
        foreach (int qty in QuotesQuantities.Where(x => x <= 502))
        {
            IEnumerable<Quote> quotes = TestData.GetDefault(qty);
            IEnumerable<StochRsiResult> r = quotes.GetStochRsi(14, 14, 3, 1);

            StochRsiResult l = r.LastOrDefault();
            Console.WriteLine(
                "SRSI on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.StochRsi);
        }
    }

    [TestMethod]
    public void T3()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<T3Result> r = quotes.GetT3(20);

            T3Result l = r.LastOrDefault();
            Console.WriteLine(
                "T3 on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.T3);
        }
    }

    [TestMethod]
    public void Tema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<TemaResult> r = quotes.GetTema(15);

            TemaResult l = r.LastOrDefault();
            Console.WriteLine(
                "TEMA on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Tema);
        }
    }

    [TestMethod]
    public void Trix()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<TrixResult> r = quotes.GetTrix(15);

            TrixResult l = r.LastOrDefault();
            Console.WriteLine(
                "TRIX on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Trix);
        }
    }

    [TestMethod]
    public void Tsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(20 + qty);
            IEnumerable<TsiResult> r = quotes.GetTsi();

            TsiResult l = r.LastOrDefault();
            Console.WriteLine(
                "TSI on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Tsi);
        }
    }

    [TestMethod]
    public void Vortex()
    {
        foreach (int qty in QuotesQuantities)
        {
            IEnumerable<Quote> quotes = TestData.GetLongish(qty);
            IEnumerable<VortexResult> r = quotes.GetVortex(14);

            VortexResult l = r.LastOrDefault();
            Console.WriteLine(
                "VI+ on {0:d} with {1,4} periods: {2:N8}",
                l.Date, quotes.Count(), l.Pvi);
        }
    }
}
