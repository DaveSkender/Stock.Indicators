using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // WEIGHTED MOVING AVERAGE
        public static IEnumerable<WmaResult> GetWma(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // clean quotes
            List<Quote> historyList = Cleaners.PrepareHistory(history).ToList();

            // check parameters
            ValidateWma(history, lookbackPeriod);

            // initialize
            List<WmaResult> results = new List<WmaResult>();
            decimal divisor = (lookbackPeriod * (lookbackPeriod + 1)) / 2m;

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];
                int index = i + 1;

                WmaResult result = new WmaResult
                {
                    Date = h.Date
                };

                if (index >= lookbackPeriod)
                {
                    decimal wma = 0;
                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        Quote d = historyList[p];
                        wma += d.Close * (lookbackPeriod - (decimal)(index - p - 1)) / divisor;
                    }

                    result.Wma = wma;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateWma(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for WMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for WMA.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }

        }
    }

}
