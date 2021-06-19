using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // WEIGHTED MOVING AVERAGE
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<WmaResult> GetWma<TQuote>(
            this IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateWma(history, lookbackPeriod);

            // initialize
            List<WmaResult> results = new(historyList.Count);
            decimal divisor = (lookbackPeriod * (lookbackPeriod + 1)) / 2m;

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                WmaResult result = new()
                {
                    Date = h.Date
                };

                if (index >= lookbackPeriod)
                {
                    decimal wma = 0;
                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        TQuote d = historyList[p];
                        wma += d.Close * (lookbackPeriod - (decimal)(index - p - 1)) / divisor;
                    }

                    result.Wma = wma;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateWma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for WMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for WMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
