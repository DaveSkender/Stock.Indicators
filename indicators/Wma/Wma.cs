using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // WEIGHTED MOVING AVERAGE
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<WmaResult> GetWma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateWma(quotes, lookbackPeriods);

            // initialize
            List<WmaResult> results = new(quotesList.Count);
            decimal divisor = (lookbackPeriods * (lookbackPeriods + 1)) / 2m;

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                WmaResult result = new()
                {
                    Date = q.Date
                };

                if (index >= lookbackPeriods)
                {
                    decimal wma = 0;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        TQuote d = quotesList[p];
                        wma += d.Close * (lookbackPeriods - (decimal)(index - p - 1)) / divisor;
                    }

                    result.Wma = wma;
                }

                results.Add(result);
            }

            return results;
        }


        // remove recommended periods extensions
        public static IEnumerable<WmaResult> RemoveWarmupPeriods(
            this IEnumerable<WmaResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Wma != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateWma<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for WMA.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for WMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
