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
            this IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateHma(history, lookbackPeriod);

            // initialize
            List<Quote> synthHistory = new();

            List<WmaResult> wmaN1 = GetWma(history, lookbackPeriod).ToList();
            List<WmaResult> wmaN2 = GetWma(history, lookbackPeriod / 2).ToList();

            // roll through history, to get interim synthetic history
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
            int sqN = (int)Math.Sqrt(lookbackPeriod);
            int shiftQty = lookbackPeriod - 1;

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


        private static void ValidateHma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for HMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for HMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
