using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // MOVING AVERAGE CONVERGENCE/DIVERGENCE (MACD) OSCILLATOR
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<MacdResult> GetMacd<TQuote>(
            this IEnumerable<TQuote> quotes,
            int fastPeriods = 12,
            int slowPeriods = 26,
            int signalPeriods = 9)
            where TQuote : IQuote
        {

            // convert quotes to basic format
            List<BasicDouble> bdList = quotes.ConvertToBasicDouble(CandlePart.Close);

            // check parameter arguments
            ValidateMacd(quotes, fastPeriods, slowPeriods, signalPeriods);

            // initialize
            List<EmaResult> emaFast = CalcEma(bdList, fastPeriods);
            List<EmaResult> emaSlow = CalcEma(bdList, slowPeriods);

            int size = bdList.Count;
            List<BasicDouble> emaDiff = new();
            List<MacdResult> results = new(size);

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                BasicDouble h = bdList[i];
                EmaResult df = emaFast[i];
                EmaResult ds = emaSlow[i];

                MacdResult result = new()
                {
                    Date = h.Date,
                    FastEma = df.Ema,
                    SlowEma = ds.Ema
                };

                if (df?.Ema != null && ds?.Ema != null)
                {

                    double macd = (double)(df.Ema - ds.Ema);
                    result.Macd = (decimal)macd;

                    // temp data for interim EMA of macd
                    BasicDouble diff = new()
                    {
                        Date = h.Date,
                        Value = macd
                    };

                    emaDiff.Add(diff);
                }

                results.Add(result);
            }

            // add signal and histogram to result
            List<EmaResult> emaSignal = CalcEma(emaDiff, signalPeriods);

            for (int d = slowPeriods - 1; d < size; d++)
            {
                MacdResult r = results[d];
                EmaResult ds = emaSignal[d + 1 - slowPeriods];

                r.Signal = ds.Ema;
                r.Histogram = r.Macd - r.Signal;
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<MacdResult> RemoveWarmupPeriods(
            this IEnumerable<MacdResult> results)
        {
            int n = results
                .ToList()
                .FindIndex(x => x.Signal != null) + 2;

            return results.Remove(n + 250);
        }


        // parameter validation
        private static void ValidateMacd<TQuote>(
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
                    "Fast periods must be greater than 0 for MACD.");
            }

            if (signalPeriods < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                    "Signal periods must be greater than or equal to 0 for MACD.");
            }

            if (slowPeriods <= fastPeriods)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                    "Slow periods must be greater than the fast period for MACD.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = Math.Max(2 * (slowPeriods + signalPeriods), slowPeriods + signalPeriods + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for MACD.  " +
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
