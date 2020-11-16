using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RATE OF CHANGE (ROC)
        public static IEnumerable<RocResult> GetRoc(
            IEnumerable<Quote> history, int lookbackPeriod, int? smaPeriod = null)
        {

            // clean quotes
            List<Quote> historyList = Cleaners.PrepareHistory(history).ToList();

            // check parameters
            ValidateRoc(history, lookbackPeriod, smaPeriod);

            // initialize
            List<RocResult> results = new List<RocResult>();

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];
                int index = i + 1;

                RocResult result = new RocResult
                {
                    Date = h.Date
                };

                if (index > lookbackPeriod)
                {
                    Quote back = historyList[index - lookbackPeriod - 1];
                    result.Roc = 100 * (h.Close - back.Close) / back.Close;
                }

                results.Add(result);

                // optional SMA
                if (smaPeriod != null && index >= lookbackPeriod + smaPeriod)
                {
                    decimal sumSma = 0m;
                    for (int p = index - (int)smaPeriod; p < index; p++)
                    {
                        sumSma += (decimal)results[p].Roc;
                    }

                    result.Sma = sumSma / smaPeriod;
                }
            }

            return results;
        }


        private static void ValidateRoc(IEnumerable<Quote> history, int lookbackPeriod, int? smaPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for ROC.");
            }

            if (smaPeriod != null && smaPeriod <= 0)
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
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }

        }
    }

}
