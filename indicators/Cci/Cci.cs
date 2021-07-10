using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // COMMODITY CHANNEL INDEX
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<CciResult> GetCci<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 20)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateCci(quotes, lookbackPeriods);

            // initialize
            List<CciResult> results = new(historyList.Count);

            // roll through quotes
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                CciResult result = new()
                {
                    Date = h.Date,
                    Tp = (h.High + h.Low + h.Close) / 3
                };
                results.Add(result);

                if (index >= lookbackPeriods)
                {
                    // average TP over lookback
                    decimal avgTp = 0;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        CciResult d = results[p];
                        avgTp += (decimal)d.Tp;
                    }
                    avgTp /= lookbackPeriods;

                    // average Deviation over lookback
                    decimal avgDv = 0;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        CciResult d = results[p];
                        avgDv += Math.Abs(avgTp - (decimal)d.Tp);
                    }
                    avgDv /= lookbackPeriods;

                    result.Cci = (avgDv == 0) ? null
                        : (result.Tp - avgTp) / ((decimal)0.015 * avgDv);
                }
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<CciResult> PruneWarmupPeriods(
            this IEnumerable<CciResult> results)
        {
            int prunePeriods = results
                .ToList()
                .FindIndex(x => x.Cci != null);

            return results.Prune(prunePeriods);
        }


        // parameter validation
        private static void ValidateCci<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Commodity Channel Index.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Commodity Channel Index.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(quotes), message);
            }
        }
    }
}
