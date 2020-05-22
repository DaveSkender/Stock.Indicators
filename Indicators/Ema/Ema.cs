using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // EXPONENTIAL MOVING AVERAGE
        public static IEnumerable<EmaResult> GetEma(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check data
            int qtyHistory = history.Count();
            int minHistory = 2 * lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for EMA.  " +
                        string.Format("You provided {0} periods of history when {1} is required.  "
                          + "Since there is smoothing, we recommend {2} periods when using lookback period of {3}, "
                          + "for maximum precision.", qtyHistory, minHistory, minHistory + 250, lookbackPeriod));
            }

            // initialize results
            List<EmaResult> results = new List<EmaResult>();

            // initialize EMA
            decimal k = 2 / (decimal)(lookbackPeriod + 1);
            decimal lastEma = history
                .Where(x => x.Index < lookbackPeriod)
                .Select(x => x.Close)
                .Average();

            // roll through history
            foreach (Quote h in history)
            {

                EmaResult result = new EmaResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod)
                {
                    result.Ema = lastEma + k * (h.Close - lastEma);
                    lastEma = (decimal)result.Ema;
                }

                results.Add(result);
            }

            return results;
        }

    }
}
