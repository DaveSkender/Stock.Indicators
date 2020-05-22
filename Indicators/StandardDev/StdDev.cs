using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // Standard Deviation
        public static IEnumerable<StdDevResult> GetStdDev(IEnumerable<Quote> history, int lookbackPeriod)
        {
            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check exceptions
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Standard Deviation.  " +
                        string.Format("You provided {0} periods of history when {1} is required.", qtyHistory, minHistory));
            }

            // initialize results
            List<StdDevResult> results = new List<StdDevResult>();


            // roll through history and compute lookback standard deviation
            foreach (Quote h in history)
            {
                StdDevResult result = new StdDevResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod)
                {
                    IEnumerable<double> period = history
                        .Where(x => x.Index <= h.Index && x.Index > (h.Index - lookbackPeriod))
                        .Select(x => (double)x.Close);

                    result.StdDev = (decimal)Functions.StdDev(period);
                }

                results.Add(result);
            }

            return results;
        }

    }

}
