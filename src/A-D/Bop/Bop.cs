using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // BALANCE OF POWER
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<BopResult> GetBop<TQuote>(
            this IEnumerable<TQuote> quotes,
            int smoothPeriods = 14)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateBop(quotes, smoothPeriods);

            // initialize
            int size = quotesList.Count;
            List<BopResult> results = new(size);

            decimal?[] raw = quotesList
                .Select(x => (x.High != x.Low) ?
                    (x.Close - x.Open) / (x.High - x.Low) : (decimal?)null)
                .ToArray();

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                BopResult r = new()
                {
                    Date = quotesList[i].Date
                };

                if (i >= smoothPeriods - 1)
                {
                    decimal? sum = 0m;
                    for (int p = i - smoothPeriods + 1; p <= i; p++)
                    {
                        sum += raw[p];
                    }

                    r.Bop = sum / smoothPeriods;
                }

                results.Add(r);
            }
            return results;
        }


        // remove recommended periods
        /// <include file='../../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<BopResult> RemoveWarmupPeriods(
            this IEnumerable<BopResult> results)
        {

            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Bop != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateBop<TQuote>(
            IEnumerable<TQuote> quotes,
            int smoothPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (smoothPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                    "Smoothing periods must be greater than 0 for BOP.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = smoothPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for BOP.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
