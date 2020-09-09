using System;
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
            Cleaners.PrepareHistory(history);

            // check parameters
            ValidateSma(history, lookbackPeriod);

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
                    List<Quote> period = history
                        .Where(x => x.Index <= h.Index && x.Index > (h.Index - lookbackPeriod))
                        .ToList();

                    // simple moving average
                    result.Sma = period
                        .Select(x => x.Close)
                        .Average();

                    // mean absolute deviation
                    result.Mad = period
                        .Select(x => Math.Abs(x.Close - (decimal)result.Sma))
                        .Average();

                    // mean squared error
                    result.Mse = period
                        .Select(x => (x.Close - (decimal)result.Sma) * (x.Close - (decimal)result.Sma))
                        .Average();

                    // mean absolute percent error
                    result.Mape = period
                        .Where(x => x.Close != 0)
                        .Select(x => Math.Abs(x.Close - (decimal)result.Sma) / x.Close)
                        .Average();
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateSma(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for SMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for SMA.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }

        }
    }

}
