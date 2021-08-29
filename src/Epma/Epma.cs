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
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateEpma(quotes, lookbackPeriods);

            // initialize
            List<SlopeResult> slopeResults = GetSlope(quotes, lookbackPeriods)
                .ToList();

            int size = slopeResults.Count;
            List<EpmaResult> results = new(size);

            // roll through quotes
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


        // remove recommended periods
        /// <include file='../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<EpmaResult> RemoveWarmupPeriods(
            this IEnumerable<EpmaResult> results)
        {
            int removePeriods = results
              .ToList()
              .FindIndex(x => x.Epma != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateEpma<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Epma.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Epma.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
