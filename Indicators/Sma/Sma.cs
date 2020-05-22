using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE
        public static IEnumerable<SmaResult> GetSma(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check exceptions
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for SMA.  " +
                        string.Format("You provided {0} periods of history when {1} is required.", qtyHistory, minHistory));
            }

            // initialize
            List<SmaResult> results = new List<SmaResult>();

            // roll through history
            foreach (Quote h in history)
            {

                SmaResult result = new SmaResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod)
                {
                    IEnumerable<decimal> period = history
                        .Where(x => x.Index <= h.Index && x.Index > (h.Index - lookbackPeriod))
                        .Select(x => x.Close);

                    result.Sma = period.Average();
                }

                results.Add(result);
            }

            return results;
        }

    }

}
