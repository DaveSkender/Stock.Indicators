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
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateEpma(history, lookbackPeriod);

            // initialize
            List<SlopeResult> slopeResults = GetSlope(history, lookbackPeriod)
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

        private static void ValidateEpma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Epma.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
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
