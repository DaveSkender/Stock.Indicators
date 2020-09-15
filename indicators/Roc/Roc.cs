using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RATE OF CHANGE (ROC)
        public static IEnumerable<RocResult> GetRoc(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // clean quotes
            Cleaners.PrepareHistory(history);

            // check parameters
            ValidateRoc(history, lookbackPeriod);

            // initialize
            List<Quote> historyList = history.ToList();
            List<RocResult> results = new List<RocResult>();

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];

                RocResult result = new RocResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index > lookbackPeriod)
                {
                    Quote back = historyList[(int)h.Index - lookbackPeriod - 1];
                    result.Roc = 100 * (h.Close - back.Close) / back.Close;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateRoc(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for ROC.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for ROC.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }

        }
    }

}
