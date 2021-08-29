using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE
        /// <include file='./info.xml' path='indicators/type[@name="Main"]/*' />
        /// 
        public static IEnumerable<SmaResult> GetSma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateSma(quotes, lookbackPeriods);

            // initialize
            List<SmaResult> results = new(quotesList.Count);

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                SmaResult result = new()
                {
                    Date = q.Date
                };

                if (index >= lookbackPeriods)
                {
                    decimal sumSma = 0m;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        TQuote d = quotesList[p];
                        sumSma += d.Close;
                    }

                    result.Sma = sumSma / lookbackPeriods;
                }

                results.Add(result);
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        /// 
        public static IEnumerable<SmaResult> RemoveWarmupPeriods(
            this IEnumerable<SmaResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Sma != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateSma<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for SMA.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for SMA.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
