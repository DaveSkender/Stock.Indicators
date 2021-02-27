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
            List<SmmaResult> results = new List<SmmaResult>(historyList.Count);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                SmmaResult result = new SmmaResult
                {
                    Date = h.Date
                };

                // first SMMA calculated as simple SMA
                if (index == lookbackPeriod)
                {
                    decimal sumSma = 0m;
                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        TQuote d = historyList[p];
                        sumSma += d.Close;
                    }

                    result.Smma = sumSma / lookbackPeriod;
                }
                // second SMMA calculated with distinct formula
                else if (index == lookbackPeriod + 1)
                {
                    decimal prevValue = results[i - 1].Smma.Value;
                    result.Smma = (prevValue * (lookbackPeriod - 1) + h.Close) / lookbackPeriod;
                }
                // remaining SMMA's calculated with base formula
                else if (index >= lookbackPeriod + 2)
                {
                    decimal prevValue = results[i - 1].Smma.Value;
                    decimal prevSum = prevValue * lookbackPeriod;
                    result.Smma = (prevSum - prevValue + h.Close) / lookbackPeriod;
                }

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
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for SMMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
