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
            IEnumerable<TQuote> history,
            int smoothPeriod = 14)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateBop(history, smoothPeriod);

            // initialize
            int size = historyList.Count;
            List<BopResult> results = new List<BopResult>(size);
            decimal? sumRaw = 0;

            decimal?[] raw = historyList
                .Select(x => (x.High != x.Low) ?
                    (x.Close - x.Open) / (x.High - x.Low) : (decimal?)null)
                .ToArray();

            // roll through history
            for (int i = 0; i < size; i++)
            {
                BopResult r = new BopResult
                {
                    Date = historyList[i].Date
                };

                sumRaw += raw[i];

                if (i >= smoothPeriod - 1)
                {
                    if (i >= smoothPeriod)
                    {
                        sumRaw -= raw[i - smoothPeriod];
                    }

                    r.Bop = sumRaw / smoothPeriod;
                }

                results.Add(r);
            }
            return results;
        }


        private static void ValidateBop<TQuote>(
            IEnumerable<TQuote> history,
            int smoothPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (smoothPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothPeriod), smoothPeriod,
                    "Smoothing period must be greater than 0 for BOP.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = smoothPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for BOP.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
