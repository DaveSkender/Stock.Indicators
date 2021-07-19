using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // HURST EXPONENT
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<HurstResult> GetHurst<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 20)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateHurst(quotes, lookbackPeriods);

            // initialize
            int size = quotesList.Count;
            List<HurstResult> results = new(size);

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                int index = i + 1;
                TQuote q = quotesList[i];

                HurstResult result = new()
                {
                    Date = q.Date
                };

                if (index >= lookbackPeriods)
                {

                }
                results.Add(result);
            }

            return results;
        }


        // remove recommended periods extensions
        public static IEnumerable<HurstResult> RemoveWarmupPeriods(
            this IEnumerable<HurstResult> results)
        {
            int removePeriods = results
              .ToList()
              .FindIndex(x => x.HurstExponent != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateHurst<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Hurst Exponent.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Hurst Exponent.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
