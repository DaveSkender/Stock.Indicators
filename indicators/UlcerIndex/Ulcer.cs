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
            List<Quote> historyList = Cleaners.PrepareHistory(history).ToList();

            // validate parameters
            ValidateUlcer(history, lookbackPeriod);

            // initialize
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
                    double sumSquared = 0;
                    for (int p = (int)h.Index - lookbackPeriod; p < h.Index; p++)
                    {
                        Quote d = historyList[p];

                        decimal maxClose = 0;
                        for (int q = (int)h.Index - lookbackPeriod; q < d.Index; q++)
                        {
                            Quote dd = historyList[q];
                            if (dd.Close > maxClose)
                            {
                                maxClose = dd.Close;
                            }
                        }

                        decimal percentDrawdown = 100 * (d.Close - maxClose) / maxClose;
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
