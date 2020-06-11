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

            // validate inputs
            ValidateStdDev(history, lookbackPeriod);

            // initialize results
            List<StdDevResult> results = new List<StdDevResult>();
            decimal? prevClose = null;

            // roll through history and compute lookback standard deviation
            foreach (Quote h in history)
            {
                StdDevResult result = new StdDevResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                };

                if (h.Index >= lookbackPeriod)
                {
                    // price based
                    IEnumerable<double> period = history
                        .Where(x => x.Index > (h.Index - lookbackPeriod) && x.Index <= h.Index)
                        .Select(x => (double)x.Close);

                    result.StdDev = (decimal)Functions.StdDev(period);
                    result.ZScore = (h.Close - (decimal)period.Average()) / result.StdDev;
                }

                results.Add(result);
                prevClose = h.Close;
            }


            // add additional values
            foreach (StdDevResult r in results.Where(x => x.Index >= lookbackPeriod + 1))
            {
                IEnumerable<StdDevResult> period = results
                    .Where(x => x.Index > (r.Index - lookbackPeriod) && x.Index <= r.Index);

            }

            return results;
        }


        private static void ValidateStdDev(IEnumerable<Quote> history, int lookbackPeriod)
        {
            if (lookbackPeriod <= 1)
            {
                throw new BadParameterException("Lookback period must be greater than 1 for Standard Deviation.");
            }

            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Standard Deviation.  " +
                        string.Format("You provided {0} periods of history when {1} is required.", qtyHistory, minHistory));
            }
        }
    }



}
