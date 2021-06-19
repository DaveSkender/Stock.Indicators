using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ElderRay
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<ElderRayResult> GetElderRay<TQuote>(
            this IEnumerable<TQuote> history,
            int lookbackPeriod = 13)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateElderRay(history, lookbackPeriod);

            // initialize with EMA
            List<ElderRayResult> results = GetEma(history, lookbackPeriod)
                .Select(x => new ElderRayResult
                {
                    Date = x.Date,
                    Ema = x.Ema
                })
                .ToList();

            // roll through history
            for (int i = lookbackPeriod - 1; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                ElderRayResult r = results[i];

                r.BullPower = h.High - r.Ema;
                r.BearPower = h.Low - r.Ema;
            }

            return results;
        }


        private static void ValidateElderRay<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Elder-ray Index.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(2 * lookbackPeriod, lookbackPeriod + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Elder-ray Index.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for a lookback period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriod, lookbackPeriod + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
