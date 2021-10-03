using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RATE OF CHANGE (ROC) WITH BANDS
        /// <include file='./info.xml' path='indicator/type[@name="WithBands"]/*' />
        /// 
        public static IEnumerable<RocWbResult> GetRocWb<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            int emaPeriods,
            int stdDevPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateRocWb(quotes, lookbackPeriods, emaPeriods, stdDevPeriods);

            // initialize
            List<RocWbResult> results = GetRoc(quotes, lookbackPeriods)
                .Select(x => new RocWbResult
                {
                    Date = x.Date,
                    Roc = x.Roc
                })
                .ToList();

            decimal k = 2m / (emaPeriods + 1);
            decimal? lastEma = 0;

            for (int i = lookbackPeriods; i < lookbackPeriods + emaPeriods; i++)
            {
                lastEma += results[i].Roc;
            }
            lastEma /= emaPeriods;

            double?[] rocSq = results
                .Select(x => (double?)(x.Roc * x.Roc))
                .ToArray();

            // roll through quotes
            for (int i = lookbackPeriods; i < results.Count; i++)
            {
                RocWbResult r = results[i];
                int index = i + 1;

                // exponential moving average
                if (index > lookbackPeriods + emaPeriods)
                {
                    r.RocEma = lastEma + k * (r.Roc - lastEma);
                    lastEma = r.RocEma;
                }
                else if (index == lookbackPeriods + emaPeriods)
                {
                    r.RocEma = lastEma;
                }

                // ROC deviation
                if (index >= lookbackPeriods + stdDevPeriods)
                {
                    double? sumSq = 0;
                    for (int p = i - stdDevPeriods + 1; p <= i; p++)
                    {
                        sumSq += rocSq[p];
                    }

                    if (sumSq is not null)
                    {
                        decimal? rocDev = (decimal?)Math.Sqrt((double)sumSq / stdDevPeriods);

                        r.UpperBand = rocDev;
                        r.LowerBand = -rocDev;
                    }
                }
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<RocWbResult> RemoveWarmupPeriods(
            this IEnumerable<RocWbResult> results)
        {
            int n = results
                .ToList()
                .FindIndex(x => x.RocEma != null) + 1;

            return results.Remove(n + 100);
        }


        // parameter validation
        private static void ValidateRocWb<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            int emaPeriods,
            int stdDevPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for ROC with Bands.");
            }

            if (emaPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(emaPeriods), emaPeriods,
                    "EMA periods must be greater than 0 for ROC.");
            }

            if (stdDevPeriods <= 0 || stdDevPeriods > lookbackPeriods)
            {
                throw new ArgumentOutOfRangeException(nameof(stdDevPeriods), stdDevPeriods,
                    "Standard Deviation periods must be greater than 0 and not more than lookback period for ROC with Bands.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for ROC with Bands.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
