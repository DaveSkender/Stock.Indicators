using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE
        /// <include file='./info.xml' path='indicators/type[@name="Main"]/*' />
        /// 
        public static IEnumerable<SmaResult> GetSma<TQuote>(
            this IEnumerable<TQuote> history,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateSma(history, lookbackPeriods);

            // initialize
            List<SmaResult> results = new(historyList.Count);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                SmaResult result = new()
                {
                    Date = h.Date
                };

                if (index >= lookbackPeriods)
                {
                    decimal sumSma = 0m;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        TQuote d = historyList[p];
                        sumSma += d.Close;
                    }

                    result.Sma = sumSma / lookbackPeriods;
                }

                results.Add(result);
            }

            return results;
        }


        // prune recommended periods extensions (standard)
        public static IEnumerable<SmaResult> PruneWarmupPeriods(
            this IEnumerable<SmaResult> results)
        {
            int prunePeriods = results
                .ToList()
                .FindIndex(x => x.Sma != null);

            return results.Prune(prunePeriods);
        }


        // parameter validation
        private static void ValidateSma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for SMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for SMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
