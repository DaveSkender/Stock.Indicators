using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SMOOTHED MOVING AVERAGE
        /// <include file='./info.xml' path='indicators/type[@name="Main"]/*' />
        ///
        public static IEnumerable<SmmaResult> GetSmma<TQuote>(
            this IEnumerable<TQuote> history,
            int lookbackPeriods)
            where TQuote : IQuote
        {
            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateSmma(history, lookbackPeriods);

            // initialize
            List<SmmaResult> results = new(historyList.Count);
            decimal? prevValue = null;

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                SmmaResult result = new()
                {
                    Date = h.Date
                };

                // calculate SMMA
                if (index > lookbackPeriods)
                {
                    result.Smma = (prevValue * (lookbackPeriods - 1) + h.Close) / lookbackPeriods;
                }

                // first SMMA calculated as simple SMA
                else if (index == lookbackPeriods)
                {
                    decimal sumClose = 0m;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        TQuote d = historyList[p];
                        sumClose += d.Close;
                    }

                    result.Smma = sumClose / lookbackPeriods;
                }

                prevValue = result.Smma;
                results.Add(result);
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<SmmaResult> PruneWarmupPeriods(
            this IEnumerable<SmmaResult> results)
        {
            int n = results
                .ToList()
                .FindIndex(x => x.Smma != null) + 1;

            return results.Prune(n + 100);
        }


        // parameter validation
        private static void ValidateSmma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriods)
            where TQuote : IQuote
        {
            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for SMMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(2 * lookbackPeriods, lookbackPeriods + 100);

            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for SMMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for {2} lookback periods "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods, lookbackPeriods + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
