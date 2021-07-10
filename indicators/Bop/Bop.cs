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
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateBop(quotes, smoothPeriods);

            // initialize
            int size = historyList.Count;
            List<BopResult> results = new(size);

            decimal?[] raw = historyList
                .Select(x => (x.High != x.Low) ?
                    (x.Close - x.Open) / (x.High - x.Low) : (decimal?)null)
                .ToArray();

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                BopResult r = new()
                {
                    Date = historyList[i].Date
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


        // prune recommended periods extensions
        public static IEnumerable<BopResult> PruneWarmupPeriods(
            this IEnumerable<BopResult> results)
        {

            int prunePeriods = results
                .ToList()
                .FindIndex(x => x.Bop != null);

            return results.Prune(prunePeriods);
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
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(quotes), message);
            }
        }
    }
}
