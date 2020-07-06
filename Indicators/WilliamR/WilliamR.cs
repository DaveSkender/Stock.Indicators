using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // WILLIAM %R OSCILLATOR
        public static IEnumerable<WilliamResult> GetWilliamR(IEnumerable<Quote> history, int lookbackPeriod = 14)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // validate parameters
            ValidateWilliam(history, lookbackPeriod);

            // initialize
            List<WilliamResult> results = new List<WilliamResult>();
            IEnumerable<StochResult> stoch = GetStoch(history, lookbackPeriod, 1, 1); // fast variant

            // convert Stochastic to William %R
            foreach (StochResult s in stoch)
            {
                WilliamResult result = new WilliamResult
                {
                    Index = s.Index,
                    Date = s.Date,
                    WilliamR = s.Oscillator - 100,
                    IsIncreasing = s.IsIncreasing
                };
                results.Add(result);
            }

            return results;
        }


        private static void ValidateWilliam(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for William %R.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for William %R.  " +
                        string.Format("You provided {0} periods of history when at least {1} is required.", qtyHistory, minHistory));
            }
        }
    }

}
