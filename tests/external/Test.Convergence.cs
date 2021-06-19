using System;
using System.Collections.Generic;
using System.Linq;
using Internal.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace External.Other
{
    [TestClass]
    public class Convergence : TestBase
    {
        private static readonly int[] convergeQuantities =
        new int[] { 5, 20, 30, 50, 75, 100, 120, 150, 200, 250, 350, 500, 600, 700, 800, 900, 1000 };

        [TestMethod]
        public void Adx()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(128 + qty);
                IEnumerable<AdxResult> r = h.GetAdx();

                AdxResult l = r.LastOrDefault();
                Console.WriteLine("ADX on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Adx);
            }
        }

        [TestMethod]
        public void Atr()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(115 + qty);
                IEnumerable<AtrResult> r = Indicator.GetAtr(h);

                AtrResult l = r.LastOrDefault();
                Console.WriteLine("ATR on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Atr);
            }
        }

        [TestMethod]
        public void ChaikinOsc()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(110 + qty);
                IEnumerable<ChaikinOscResult> r = Indicator.GetChaikinOsc(h);

                ChaikinOscResult l = r.LastOrDefault();
                Console.WriteLine("CHAIKIN OSC on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Oscillator);
            }
        }

        [TestMethod]
        public void ConnorsRsi()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(103 + qty);
                IEnumerable<ConnorsRsiResult> r = Indicator.GetConnorsRsi(h, 3, 2, 10);

                ConnorsRsiResult l = r.LastOrDefault();
                Console.WriteLine("CRSI on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.ConnorsRsi);
            }
        }

        [TestMethod]
        public void DoubleEma()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(130 + qty);
                IEnumerable<EmaResult> r = Indicator.GetDoubleEma(h, 15);

                EmaResult l = r.LastOrDefault();
                Console.WriteLine("DEMA on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Ema);
            }
        }

        [TestMethod]
        public void Ema()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(115 + qty);
                IEnumerable<EmaResult> r = Indicator.GetEma(h, 15);

                EmaResult l = r.LastOrDefault();
                Console.WriteLine("EMA on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Ema);
            }
        }

        [TestMethod]
        public void FisherTransform()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(5 + qty);
                IEnumerable<FisherTransformResult> r = Indicator.GetFisherTransform(h, 10);

                FisherTransformResult l = r.LastOrDefault();
                Console.WriteLine("FT on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Fisher);
            }
        }

        [TestMethod]
        public void HtTrendline()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(100 + qty);
                IEnumerable<HtlResult> r = Indicator.GetHtTrendline(h);

                HtlResult l = r.LastOrDefault();
                Console.WriteLine("HTL on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Trendline);
            }
        }

        [TestMethod]
        public void Kama()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(105 + qty);
                IEnumerable<KamaResult> r = Indicator.GetKama(h, 10);

                KamaResult l = r.LastOrDefault();
                Console.WriteLine("KAMA on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Kama);
            }
        }

        [TestMethod]
        public void Keltner()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(200 + qty);
                IEnumerable<KeltnerResult> r = Indicator.GetKeltner(h, 100);

                KeltnerResult l = r.LastOrDefault();
                Console.WriteLine("KC-UP on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.UpperBand);
            }
        }

        [TestMethod]
        public void Macd()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(130 + qty);
                IEnumerable<MacdResult> r = Indicator.GetMacd(h);

                MacdResult l = r.LastOrDefault();
                Console.WriteLine("MACD on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Macd);
            }
        }

        [TestMethod]
        public void Mama()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(50 + qty);
                IEnumerable<MamaResult> r = Indicator.GetMama(h);

                MamaResult l = r.LastOrDefault();
                Console.WriteLine("MAMA on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Mama);
            }
        }

        [TestMethod]
        public void Pmo()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(130 + qty);
                IEnumerable<PmoResult> r = Indicator.GetPmo(h);

                PmoResult l = r.LastOrDefault();
                Console.WriteLine("PMO on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Pmo);
            }
        }

        [TestMethod]
        public void Pvo()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(130 + qty);
                IEnumerable<PvoResult> r = Indicator.GetPvo(h);

                PvoResult l = r.LastOrDefault();
                Console.WriteLine("PVO on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Pvo);
            }
        }

        [TestMethod]
        public void Rsi()
        {
            int lookbackPeriod = 14;

            foreach (int qty in convergeQuantities.Where(q => q > 100 - lookbackPeriod))
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(lookbackPeriod + qty);
                IEnumerable<RsiResult> r = Indicator.GetRsi(h, lookbackPeriod);

                RsiResult l = r.LastOrDefault();
                Console.WriteLine("RSI({0}) on {1:d} with {2,4} periods: {3:N8}",
                    lookbackPeriod, l.Date, h.Count(), l.Rsi);
            }
        }

        [TestMethod]
        public void Smma()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(110 + qty);
                IEnumerable<SmmaResult> r = Indicator.GetSmma(h, 15);

                SmmaResult l = r.LastOrDefault();
                Console.WriteLine("SMMA(15) on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Smma);
            }
        }

        [TestMethod]
        public void StarcBands()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(200 + qty);
                IEnumerable<StarcBandsResult> r = Indicator.GetStarcBands(h, 100);

                StarcBandsResult l = r.LastOrDefault();
                Console.WriteLine("STARC UPPER on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.UpperBand);
            }
        }

        [TestMethod]
        public void StochRsi()
        {
            foreach (int qty in convergeQuantities.Where(x => x <= 502))
            {
                IEnumerable<Quote> h = HistoryTestData.Get(110 + qty);
                IEnumerable<StochRsiResult> r = Indicator.GetStochRsi(h, 14, 14, 3, 1);

                StochRsiResult l = r.LastOrDefault();
                Console.WriteLine("SRSI on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.StochRsi);
            }
        }

        [TestMethod]
        public void T3()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong((6 * 20) + 100 + qty);
                IEnumerable<T3Result> r = Indicator.GetT3(h, 20);

                T3Result l = r.LastOrDefault();
                Console.WriteLine("T3 on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.T3);
            }
        }

        [TestMethod]
        public void TripleEma()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(145 + qty);
                IEnumerable<EmaResult> r = Indicator.GetTripleEma(h, 15);

                EmaResult l = r.LastOrDefault();
                Console.WriteLine("TEMA on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Ema);
            }
        }

        [TestMethod]
        public void Trix()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(140 + qty);
                IEnumerable<TrixResult> r = Indicator.GetTrix(h, 15);

                TrixResult l = r.LastOrDefault();
                Console.WriteLine("TRIX on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Trix);
            }
        }

        [TestMethod]
        public void Tsi()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(135 + qty);
                IEnumerable<TsiResult> r = Indicator.GetTsi(h);

                TsiResult l = r.LastOrDefault();
                Console.WriteLine("TSI on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Tsi);
            }
        }

        [TestMethod]
        public void Vortex()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(15 + qty);
                IEnumerable<VortexResult> r = Indicator.GetVortex(h, 14);

                VortexResult l = r.LastOrDefault();
                Console.WriteLine("VI+ on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Pvi);
            }
        }

    }
}
