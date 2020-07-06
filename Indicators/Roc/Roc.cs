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
            history = Cleaners.PrepareHistory(history);

            // check parameters
            ValidateRoc(history, lookbackPeriod);

            // initialize
            List<RocResult> results = new List<RocResult>();

            // roll through history
            foreach (Quote h in history)
            {

                RocResult result = new RocResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index > lookbackPeriod)
                {
                    Quote back = history
                        .Where(x => x.Index == h.Index - lookbackPeriod)
                        .FirstOrDefault();

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
                        string.Format("You provided {0} periods of history when at least {1} is required.", qtyHistory, minHistory));
            }

        }
    }

}
