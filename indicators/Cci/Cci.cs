using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // COMMODITY CHANNEL INDEX
        public static IEnumerable<CciResult> GetCci(IEnumerable<Quote> history, int lookbackPeriod = 20)
        {

            // clean quotes
            List<Quote> historyList = Cleaners.PrepareHistory(history).ToList();

            // validate parameters
            ValidateCci(history, lookbackPeriod);

            // initialize
            List<CciResult> results = new List<CciResult>();


            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];

                CciResult result = new CciResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                    Tp = (h.High + h.Low + h.Close) / 3
                };
                results.Add(result);

                if (h.Index >= lookbackPeriod)
                {
                    // average TP over lookback
                    decimal avgTp = 0;
                    for (int p = (int)h.Index - lookbackPeriod; p < h.Index; p++)
                    {
                        CciResult d = results[p];
                        avgTp += (decimal)d.Tp;
                    }
                    avgTp /= lookbackPeriod;

                    // average Deviation over lookback
                    decimal avgDv = 0;
                    for (int p = (int)h.Index - lookbackPeriod; p < h.Index; p++)
                    {
                        CciResult d = results[p];
                        avgDv += Math.Abs(avgTp - (decimal)d.Tp);
                    }
                    avgDv /= lookbackPeriod;

                    result.Cci = (result.Tp - avgTp) / ((decimal)0.015 * avgDv);
                }
            }

            return results;
        }


        private static void ValidateCci(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for Commodity Channel Index.");
            }


            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Commodity Channel Index.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }
        }
    }
}
