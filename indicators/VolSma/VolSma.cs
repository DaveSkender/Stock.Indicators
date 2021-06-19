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
            this IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // sort history and initialize results
            List<VolSmaResult> results = history.Sort()
                .Select(x => new VolSmaResult
                {
                    Date = x.Date,
                    Volume = x.Volume
                })
                .ToList();

            // check parameter arguments
            ValidateVolSma(history, lookbackPeriod);

            // roll through history
            for (int i = lookbackPeriod - 1; i < results.Count; i++)
            {
                VolSmaResult h = results[i];
                int index = i + 1;

                decimal sumVolSma = 0m;
                for (int p = index - lookbackPeriod; p < index; p++)
                {
                    VolSmaResult q = results[p];
                    sumVolSma += q.Volume;
                }

                h.VolSma = sumVolSma / lookbackPeriod;
            }

            return results;
        }


        private static void ValidateVolSma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for VolSma.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for VolSma.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
