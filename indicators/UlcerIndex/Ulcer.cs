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
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 14)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateUlcer(quotes, lookbackPeriods);

            // initialize
            List<UlcerIndexResult> results = new(historyList.Count);

            // roll through quotes
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                UlcerIndexResult result = new()
                {
                    Date = h.Date
                };

                if (index >= lookbackPeriods)
                {
                    double? sumSquared = 0;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        TQuote d = historyList[p];
                        int dIndex = p + 1;

                        decimal maxClose = 0;
                        for (int q = index - lookbackPeriods; q < dIndex; q++)
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
                        : (decimal)Math.Sqrt((double)sumSquared / lookbackPeriods);
                }
                results.Add(result);
            }


            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<UlcerIndexResult> PruneWarmupPeriods(
            this IEnumerable<UlcerIndexResult> results)
        {
            int prunePeriods = results
                .ToList()
                .FindIndex(x => x.UI != null);

            return results.Prune(prunePeriods);
        }


        // parameter validation
        private static void ValidateUlcer<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Ulcer Index.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Ulcer Index.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(quotes), message);
            }
        }
    }
}
