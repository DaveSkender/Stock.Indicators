using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ElderRay
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<ElderRayResult> GetElderRay<TQuote>(
            this IEnumerable<TQuote> history,
            int lookbackPeriods = 13)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateElderRay(history, lookbackPeriods);

            // initialize with EMA
            List<ElderRayResult> results = GetEma(history, lookbackPeriods)
                .Select(x => new ElderRayResult
                {
                    Date = x.Date,
                    Ema = x.Ema
                })
                .ToList();

            // roll through history
            for (int i = lookbackPeriods - 1; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                ElderRayResult r = results[i];

                r.BullPower = h.High - r.Ema;
                r.BearPower = h.Low - r.Ema;
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<ElderRayResult> PruneWarmupPeriods(
            this IEnumerable<ElderRayResult> results)
        {
            int n = results
              .ToList()
              .FindIndex(x => x.BullPower != null) + 1;

            return results.Prune(n + 100);
        }


        // parameter validation
        private static void ValidateElderRay<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback period must be greater than 0 for Elder-ray Index.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(2 * lookbackPeriods, lookbackPeriods + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Elder-ray Index.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for a lookback period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods, lookbackPeriods + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
