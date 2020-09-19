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

                RocResult result = new RocResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index > lookbackPeriod)
                {
                    Quote back = historyList[(int)h.Index - lookbackPeriod - 1];
                    result.Roc = 100 * (h.Close - back.Close) / back.Close;
                }

                results.Add(result);

                // optional SMA
                if (smaPeriod != null && h.Index >= lookbackPeriod + smaPeriod)
                {
                    decimal sumSma = 0m;
                    for (int p = (int)h.Index - (int)smaPeriod; p < h.Index; p++)
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
                throw new BadParameterException("Lookback period must be greater than 0 for ROC.");
            }

            if (smaPeriod != null && smaPeriod <= 0)
            {
                throw new BadParameterException("SMA period must be greater than 0 for ROC.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for ROC.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }

        }
    }

}
