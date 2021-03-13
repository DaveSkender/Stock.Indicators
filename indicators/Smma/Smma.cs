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
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {
            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateSmma(history, lookbackPeriod);

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
                if (index > lookbackPeriod)
                {
                    result.Smma = (prevValue * (lookbackPeriod - 1) + h.Close) / lookbackPeriod;
                }

                // first SMMA calculated as simple SMA
                else if (index == lookbackPeriod)
                {
                    decimal sumClose = 0m;
                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        TQuote d = historyList[p];
                        sumClose += d.Close;
                    }

                    result.Smma = sumClose / lookbackPeriod;
                }

                prevValue = result.Smma;
                results.Add(result);
            }

            return results;
        }

        private static void ValidateSmma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {
            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for SMMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(2 * lookbackPeriod, lookbackPeriod + 100);

            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for SMMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for a lookback period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriod, lookbackPeriod + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
