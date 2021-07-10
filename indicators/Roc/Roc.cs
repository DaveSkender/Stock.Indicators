using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RATE OF CHANGE (ROC)
        /// <include file='./info.xml' path='indicators/type[@name="Main"]/*' />
        /// 
        public static IEnumerable<RocResult> GetRoc<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            int? smaPeriods = null)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateRoc(quotes, lookbackPeriods, smaPeriods);

            // initialize
            List<RocResult> results = new(historyList.Count);

            // roll through quotes
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                RocResult result = new()
                {
                    Date = h.Date
                };

                if (index > lookbackPeriods)
                {
                    TQuote back = historyList[index - lookbackPeriods - 1];

                    result.Roc = (back.Close == 0) ? null
                        : 100 * (h.Close - back.Close) / back.Close;
                }

                results.Add(result);

                // optional SMA
                if (smaPeriods != null && index >= lookbackPeriods + smaPeriods)
                {
                    decimal? sumSma = 0m;
                    for (int p = index - (int)smaPeriods; p < index; p++)
                    {
                        sumSma += results[p].Roc;
                    }

                    result.RocSma = sumSma / smaPeriods;
                }
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<RocResult> PruneWarmupPeriods(
            this IEnumerable<RocResult> results)
        {
            int prunePeriods = results
                .ToList()
                .FindIndex(x => x.Roc != null);

            return results.Prune(prunePeriods);
        }


        // parameter validation
        private static void ValidateRoc<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            int? smaPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for ROC.");
            }

            if (smaPeriods is not null and <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                    "SMA periods must be greater than 0 for ROC.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for ROC.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(quotes), message);
            }
        }
    }
}
