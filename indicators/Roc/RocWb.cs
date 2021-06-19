using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RATE OF CHANGE (ROC) WITH BANDS
        /// <include file='./info.xml' path='indicators/type[@name="WithBands"]/*' />
        /// 
        public static IEnumerable<RocWbResult> GetRocWb<TQuote>(
            this IEnumerable<TQuote> history,
            int lookbackPeriod,
            int emaPeriod,
            int stdDevPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateRocWb(history, lookbackPeriod, emaPeriod, stdDevPeriod);

            // initialize
            List<RocWbResult> results = GetRoc(history, lookbackPeriod)
                .Select(x => new RocWbResult
                {
                    Date = x.Date,
                    Roc = x.Roc
                })
                .ToList();

            decimal k = 2m / (emaPeriod + 1);
            decimal? lastEma = 0;

            for (int i = lookbackPeriod; i < lookbackPeriod + emaPeriod; i++)
            {
                lastEma += results[i].Roc;
            }
            lastEma /= emaPeriod;

            double?[] rocSq = results
                .Select(x => (double?)(x.Roc * x.Roc))
                .ToArray();

            // roll through history
            for (int i = lookbackPeriod; i < results.Count; i++)
            {
                RocWbResult r = results[i];
                int index = i + 1;

                // exponential moving average
                if (index > lookbackPeriod + emaPeriod)
                {
                    r.RocEma = lastEma + k * (r.Roc - lastEma);
                    lastEma = r.RocEma;
                }
                else if (index == lookbackPeriod + emaPeriod)
                {
                    r.RocEma = lastEma;
                }

                // ROC deviation
                if (index >= lookbackPeriod + stdDevPeriod)
                {
                    double? sumSq = 0;
                    for (int p = i - stdDevPeriod + 1; p <= i; p++)
                    {
                        sumSq += rocSq[p];
                    }

                    if (sumSq is not null)
                    {
                        decimal? rocDev = (decimal?)Math.Sqrt((double)sumSq / stdDevPeriod);

                        r.UpperBand = rocDev;
                        r.LowerBand = -rocDev;
                    }
                }
            }

            return results;
        }


        private static void ValidateRocWb<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod,
            int emaPeriod,
            int stdDevPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for ROC with Bands.");
            }

            if (emaPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(emaPeriod), emaPeriod,
                    "EMA period must be greater than 0 for ROC.");
            }

            if (stdDevPeriod <= 0 || stdDevPeriod > lookbackPeriod)
            {
                throw new ArgumentOutOfRangeException(nameof(stdDevPeriod), stdDevPeriod,
                    "Standard Deviation period must be greater than 0 and not more than lookback period for ROC with Bands.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for ROC with Bands.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
