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
            this IEnumerable<TQuote> history,
            int lookbackPeriod,
            int? smaPeriod = null)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateRoc(history, lookbackPeriod, smaPeriod);

            // initialize
            List<RocResult> results = new(historyList.Count);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                RocResult result = new()
                {
                    Date = h.Date
                };

                if (index > lookbackPeriod)
                {
                    TQuote back = historyList[index - lookbackPeriod - 1];

                    result.Roc = (back.Close == 0) ? null
                        : 100 * (h.Close - back.Close) / back.Close;
                }

                results.Add(result);

                // optional SMA
                if (smaPeriod != null && index >= lookbackPeriod + smaPeriod)
                {
                    decimal? sumSma = 0m;
                    for (int p = index - (int)smaPeriod; p < index; p++)
                    {
                        sumSma += results[p].Roc;
                    }

                    result.RocSma = sumSma / smaPeriod;
                }
            }

            return results;
        }


        private static void ValidateRoc<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod,
            int? smaPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for ROC.");
            }

            if (smaPeriod is not null and <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriod), smaPeriod,
                    "SMA period must be greater than 0 for ROC.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for ROC.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
