using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ENDPOINT MOVING AVERAGE
        /// <include file='./info.xml' path='indicators/type[@name="Main"]/*' />
        /// 
        public static IEnumerable<EpmaResult> GetEpma<TQuote>(
            this IEnumerable<TQuote> history,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateEpma(history, lookbackPeriods);

            // initialize
            List<SlopeResult> slopeResults = GetSlope(history, lookbackPeriods)
                .ToList();

            int size = slopeResults.Count;
            List<EpmaResult> results = new(size);

            // roll through history
            for (int i = 0; i < size; i++)
            {
                SlopeResult s = slopeResults[i];

                EpmaResult r = new()
                {
                    Date = s.Date,
                    Epma = s.Slope * (i + 1) + s.Intercept
                };

                results.Add(r);
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<EpmaResult> PruneWarmupPeriods(
            this IEnumerable<EpmaResult> results)
        {
            int prunePeriods = results
              .ToList()
              .FindIndex(x => x.Epma != null);

            return results.Prune(prunePeriods);
        }


        // parameter validation
        private static void ValidateEpma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback period must be greater than 0 for Epma.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Epma.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
