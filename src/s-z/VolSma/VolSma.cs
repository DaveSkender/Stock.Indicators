using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE of VOLUME
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<VolSmaResult> GetVolSma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // sort quotes and initialize results
            List<VolSmaResult> results = quotes.Sort()
                .Select(x => new VolSmaResult
                {
                    Date = x.Date,
                    Volume = x.Volume
                })
                .ToList();

            // check parameter arguments
            ValidateVolSma(quotes, lookbackPeriods);

            // roll through quotes
            for (int i = lookbackPeriods - 1; i < results.Count; i++)
            {
                VolSmaResult h = results[i];
                int index = i + 1;

                decimal sumVolSma = 0m;
                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    VolSmaResult q = results[p];
                    sumVolSma += q.Volume;
                }

                h.VolSma = sumVolSma / lookbackPeriods;
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<VolSmaResult> RemoveWarmupPeriods(
            this IEnumerable<VolSmaResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.VolSma != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateVolSma<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for VolSma.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for VolSma.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
