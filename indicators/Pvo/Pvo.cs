using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // PRICE VOLUME OSCILLATOR (PVO)
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<PvoResult> GetPvo<TQuote>(
            this IEnumerable<TQuote> history,
            int fastPeriod = 12,
            int slowPeriod = 26,
            int signalPeriod = 9)
            where TQuote : IQuote
        {

            // convert history to basic format
            List<BasicData> bdList = history.ConvertToBasic("V");

            // check parameter arguments
            ValidatePvo(history, fastPeriod, slowPeriod, signalPeriod);

            // initialize
            List<EmaResult> emaFast = CalcEma(bdList, fastPeriod).ToList();
            List<EmaResult> emaSlow = CalcEma(bdList, slowPeriod).ToList();

            int size = bdList.Count;
            List<BasicData> emaDiff = new();
            List<PvoResult> results = new(size);

            // roll through history
            for (int i = 0; i < size; i++)
            {
                BasicData h = bdList[i];
                EmaResult df = emaFast[i];
                EmaResult ds = emaSlow[i];

                PvoResult result = new()
                {
                    Date = h.Date
                };

                if (df?.Ema != null && ds?.Ema != null)
                {

                    decimal? pvo = (ds.Ema != 0) ?
                        100 * (df.Ema - ds.Ema) / ds.Ema : null;

                    result.Pvo = pvo;

                    // temp data for interim EMA of PVO
                    BasicData diff = new()
                    {
                        Date = h.Date,
                        Value = (decimal)pvo
                    };

                    emaDiff.Add(diff);
                }

                results.Add(result);
            }

            // add signal and histogram to result
            List<EmaResult> emaSignal = CalcEma(emaDiff, signalPeriod).ToList();

            for (int d = slowPeriod - 1; d < size; d++)
            {
                PvoResult r = results[d];
                EmaResult ds = emaSignal[d + 1 - slowPeriod];

                r.Signal = ds.Ema;
                r.Histogram = r.Pvo - r.Signal;
            }

            return results;
        }


        private static void ValidatePvo<TQuote>(
            IEnumerable<TQuote> history,
            int fastPeriod,
            int slowPeriod,
            int signalPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (fastPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fastPeriod), fastPeriod,
                    "Fast period must be greater than 0 for PVO.");
            }

            if (signalPeriod < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriod), signalPeriod,
                    "Signal period must be greater than or equal to 0 for PVO.");
            }

            if (slowPeriod <= fastPeriod)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriod), slowPeriod,
                    "Slow period must be greater than the fast period for PVO.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(2 * (slowPeriod + signalPeriod), slowPeriod + signalPeriod + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for PVO.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least {2} data points prior to the intended "
                    + "usage date for better precision.", qtyHistory, minHistory, slowPeriod + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
