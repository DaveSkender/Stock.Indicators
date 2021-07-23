using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SMOOTHED MOVING AVERAGE
        /// <include file='./info.xml' path='indicators/type[@name="Main"]/*' />
        ///
        public static IEnumerable<SmmaResult> GetSmma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {
            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateSmma(quotes, lookbackPeriods);

            // initialize
            List<SmmaResult> results = new(quotesList.Count);
            decimal? prevValue = null;

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                SmmaResult result = new()
                {
                    Date = q.Date
                };

                // calculate SMMA
                if (index > lookbackPeriods)
                {
                    result.Smma = (prevValue * (lookbackPeriods - 1) + q.Close) / lookbackPeriods;
                }

                // first SMMA calculated as simple SMA
                else if (index == lookbackPeriods)
                {
                    decimal sumClose = 0m;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        TQuote d = quotesList[p];
                        sumClose += d.Close;
                    }

                    result.Smma = sumClose / lookbackPeriods;
                }

                prevValue = result.Smma;
                results.Add(result);
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<SmmaResult> RemoveWarmupPeriods(
            this IEnumerable<SmmaResult> results)
        {
            int n = results
                .ToList()
                .FindIndex(x => x.Smma != null) + 1;

            return results.Remove(n + 100);
        }


        // parameter validation
        private static void ValidateSmma<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {
            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for SMMA.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = Math.Max(2 * lookbackPeriods, lookbackPeriods + 100);

            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for SMMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for {2} lookback periods "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods, lookbackPeriods + 250);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
