using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // HULL MOVING AVERAGE
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<HmaResult> GetHma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateHma(quotes, lookbackPeriods);

            // initialize
            List<Quote> synthHistory = new();

            List<WmaResult> wmaN1 = GetWma(quotes, lookbackPeriods).ToList();
            List<WmaResult> wmaN2 = GetWma(quotes, lookbackPeriods / 2).ToList();

            // roll through quotes, to get interim synthetic quotes
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];

                Quote sh = new()
                {
                    Date = h.Date
                };

                WmaResult w1 = wmaN1[i];
                WmaResult w2 = wmaN2[i];

                if (w1.Wma != null && w2.Wma != null)
                {
                    sh.Close = (decimal)(w2.Wma * 2m - w1.Wma);
                    synthHistory.Add(sh);
                }
            }

            // add back truncated null results
            int sqN = (int)Math.Sqrt(lookbackPeriods);
            int shiftQty = lookbackPeriods - 1;

            List<HmaResult> results = historyList
                .Take(shiftQty)
                .Select(x => new HmaResult
                {
                    Date = x.Date
                })
                .ToList();

            // calculate final HMA = WMA with period SQRT(n)
            List<HmaResult> hmaResults = GetWma(synthHistory, sqN)
                .Select(x => new HmaResult
                {
                    Date = x.Date,
                    Hma = x.Wma
                })
                .ToList();

            // add WMA to results
            results.AddRange(hmaResults);
            results = results.OrderBy(x => x.Date).ToList();

            return results;
        }


        // remove recommended periods extensions
        public static IEnumerable<HmaResult> RemoveWarmupPeriods(
            this IEnumerable<HmaResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Hma != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateHma<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 1 for HMA.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for HMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
