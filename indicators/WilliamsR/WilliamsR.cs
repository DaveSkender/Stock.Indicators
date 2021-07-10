using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // WILLIAM %R OSCILLATOR
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<WilliamsResult> GetWilliamsR<TQuote>(
            this IEnumerable<TQuote> history,
            int lookbackPeriods = 14)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateWilliam(history, lookbackPeriods);

            // convert Stochastic to William %R
            return GetStoch(history, lookbackPeriods, 1, 1) // fast variant
                .Select(s => new WilliamsResult
                {
                    Date = s.Date,
                    WilliamsR = s.Oscillator - 100
                })
                .ToList();
        }


        // prune recommended periods extensions
        public static IEnumerable<WilliamsResult> PruneWarmupPeriods(
            this IEnumerable<WilliamsResult> results)
        {
            int prunePeriods = results
                .ToList()
                .FindIndex(x => x.WilliamsR != null);

            return results.Prune(prunePeriods);
        }


        // parameter validation
        private static void ValidateWilliam<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback period must be greater than 0 for William %R.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for William %R.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
