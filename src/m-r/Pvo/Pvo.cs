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
            this IEnumerable<TQuote> quotes,
            int fastPeriods = 12,
            int slowPeriods = 26,
            int signalPeriods = 9)
            where TQuote : IQuote
        {

            // convert quotes to basic format
            List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.Volume);

            // check parameter arguments
            ValidatePvo(quotes, fastPeriods, slowPeriods, signalPeriods);

            // initialize
            List<EmaResult> emaFast = CalcEma(bdList, fastPeriods);
            List<EmaResult> emaSlow = CalcEma(bdList, slowPeriods);

            int size = bdList.Count;
            List<BasicD> emaDiff = new();
            List<PvoResult> results = new(size);

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                BasicD h = bdList[i];
                EmaResult df = emaFast[i];
                EmaResult ds = emaSlow[i];

                PvoResult result = new()
                {
                    Date = h.Date
                };

                if (df?.Ema != null && ds?.Ema != null)
                {

                    double? pvo = (ds.Ema != 0) ?
                        100 * (double)((df.Ema - ds.Ema) / ds.Ema) : null;

                    result.Pvo = (decimal?)pvo;

                    // temp data for interim EMA of PVO
                    BasicD diff = new()
                    {
                        Date = h.Date,
                        Value = (double)pvo
                    };

                    emaDiff.Add(diff);
                }

                results.Add(result);
            }

            // add signal and histogram to result
            List<EmaResult> emaSignal = CalcEma(emaDiff, signalPeriods);

            for (int d = slowPeriods - 1; d < size; d++)
            {
                PvoResult r = results[d];
                EmaResult ds = emaSignal[d + 1 - slowPeriods];

                r.Signal = ds.Ema;
                r.Histogram = r.Pvo - r.Signal;
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<PvoResult> RemoveWarmupPeriods(
            this IEnumerable<PvoResult> results)
        {
            int n = results
                .ToList()
                .FindIndex(x => x.Signal != null) + 2;

            return results.Remove(n + 250);
        }


        // parameter validation
        private static void ValidatePvo<TQuote>(
            IEnumerable<TQuote> quotes,
            int fastPeriods,
            int slowPeriods,
            int signalPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (fastPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                    "Fast periods must be greater than 0 for PVO.");
            }

            if (signalPeriods < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                    "Signal periods must be greater than or equal to 0 for PVO.");
            }

            if (slowPeriods <= fastPeriods)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                    "Slow periods must be greater than the fast period for PVO.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = Math.Max(2 * (slowPeriods + signalPeriods), slowPeriods + signalPeriods + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for PVO.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least {2} data points prior to the intended "
                    + "usage date for better precision.", qtyHistory, minHistory, slowPeriods + 250);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
