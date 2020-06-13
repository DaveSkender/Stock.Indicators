using System;
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

            // validate parameters
            ValidateEma(history, lookbackPeriod);

            // initialize
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


        private static void ValidateEma(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for EMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(2 * lookbackPeriod, lookbackPeriod + 100);
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for EMA.  " +
                        string.Format("You provided {0} periods of history when at least {1} is required.  "
                          + "Since this uses a smoothing technique, for a lookback period of {2}, "
                          + "we recommend you use at least {3} data points prior to the intended "
                          + "usage date for maximum precision.",
                          qtyHistory, minHistory, lookbackPeriod, 2 * lookbackPeriod + 250));
            }

        }
    }
}
