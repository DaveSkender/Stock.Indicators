using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ULCER INDEX (UI)
        public static IEnumerable<UlcerIndexResult> GetUlcerIndex(IEnumerable<Quote> history, int lookbackPeriod = 14)
        {

            // clean quotes
            Cleaners.PrepareHistory(history);

            // validate parameters
            ValidateUlcer(history, lookbackPeriod);

            // initialize
            List<Quote> historyList = history.ToList();
            List<UlcerIndexResult> results = new List<UlcerIndexResult>();

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];

                UlcerIndexResult result = new UlcerIndexResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod)
                {
                    List<Quote> period = historyList
                        .Where(x => x.Index > (h.Index - lookbackPeriod) && x.Index <= h.Index)
                        .ToList();

                    double sumSquared = 0;

                    foreach (Quote p in period)
                    {
                        decimal maxClose = period
                            .Where(x => x.Index <= p.Index)
                            .Select(x => x.Close)
                            .Max();

                        decimal percentDrawdown = 100 * (p.Close - maxClose) / maxClose;

                        sumSquared += (double)(percentDrawdown * percentDrawdown);
                    }

                    result.UI = (decimal)Math.Sqrt(sumSquared / lookbackPeriod);
                }
                results.Add(result);
            }


            return results;
        }

        private static void ValidateUlcer(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for Ulcer Index.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Ulcer Index.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }

        }
    }

}
