using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE of VOLUME
        // DO NOT USE - WILL BE DEPRECATED AT END OF 2021
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<VolSmaResult> GetVolSma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            List<TQuote> quotesList = quotes.SortToList();

            // check parameter arguments
            ValidateVolSma(quotes, lookbackPeriods);

            // initialize
            int size = quotesList.Count;
            List<VolSmaResult> results = new(size);
            List<SmaResult> sma = quotes
                .GetSma(lookbackPeriods, CandlePart.Volume)
                .ToList();

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                SmaResult s = sma[i];
                TQuote q = quotesList[i];

                results.Add(new VolSmaResult
                {
                    Date = s.Date,
                    VolSma = s.Sma,
                    Volume = q.Volume
                });
            }

            string msg = "WARNING! This indicator will be replaced by GetSma() "
                       + "and removed at the end of 2021 in future versions. "
                       + "Please migrate your scripts now.";
            Console.WriteLine(msg);

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
