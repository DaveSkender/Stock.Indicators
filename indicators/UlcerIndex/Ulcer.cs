using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ULCER INDEX (UI)
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<UlcerIndexResult> GetUlcerIndex<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod = 14)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateUlcer(history, lookbackPeriod);

            // initialize
            List<UlcerIndexResult> results = new List<UlcerIndexResult>(historyList.Count);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                UlcerIndexResult result = new UlcerIndexResult
                {
                    Date = h.Date
                };

                if (index >= lookbackPeriod)
                {
                    double? sumSquared = 0;
                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        TQuote d = historyList[p];
                        int dIndex = p + 1;

                        decimal maxClose = 0;
                        for (int q = index - lookbackPeriod; q < dIndex; q++)
                        {
                            TQuote dd = historyList[q];
                            if (dd.Close > maxClose)
                            {
                                maxClose = dd.Close;
                            }
                        }

                        decimal? percentDrawdown = (maxClose == 0) ? null
                            : 100 * (d.Close - maxClose) / maxClose;

                        sumSquared += (double?)(percentDrawdown * percentDrawdown);
                    }

                    result.UI = (sumSquared == null) ? null
                        : (decimal)Math.Sqrt((double)sumSquared / lookbackPeriod);
                }
                results.Add(result);
            }


            return results;
        }


        private static void ValidateUlcer<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Ulcer Index.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Ulcer Index.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
