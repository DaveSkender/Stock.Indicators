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

                WmaResult result = new WmaResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod)
                {
                    decimal wma = 0;
                    for (int p = (int)h.Index - lookbackPeriod; p < h.Index; p++)
                    {
                        Quote d = historyList[p];
                        wma += d.Close * (lookbackPeriod - (decimal)(h.Index - d.Index)) / divisor;
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
                throw new BadParameterException("Lookback period must be greater than 0 for WMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for WMA.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }

        }
    }

}
