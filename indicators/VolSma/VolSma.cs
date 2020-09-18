using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE of VOLUME
        public static IEnumerable<VolSmaResult> GetVolSma(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // clean quotes and initialize results
            List<VolSmaResult> results = Cleaners.PrepareHistory(history)
                .Select(x => new VolSmaResult
                {
                    Index = x.Index,
                    Date = x.Date,
                    Open = x.Open,
                    High = x.High,
                    Low = x.Low,
                    Close = x.Close,
                    Volume = x.Volume
                })
                .ToList();

            // check parameters
            ValidateVolSma(history, lookbackPeriod);

            // roll through history
            for (int i = lookbackPeriod - 1; i < results.Count; i++)
            {
                VolSmaResult h = results[i];

                decimal sumVolSma = 0m;
                for (int p = (int)h.Index - lookbackPeriod; p < h.Index; p++)
                {
                    VolSmaResult q = results[p];
                    sumVolSma += q.Volume;
                }

                h.VolSma = sumVolSma / lookbackPeriod;
            }

            return results;
        }


        private static void ValidateVolSma(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for VolSma.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for VolSma.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }

        }
    }

}
