using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ARNAUD LEGOUX MOVING AVERAGE
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<AlmaResult> GetAlma<TQuote>(
            this IEnumerable<TQuote> history,
            int lookbackPeriods = 9,
            double offset = 0.85,
            double sigma = 6)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateAlma(history, lookbackPeriods, offset, sigma);

            // initialize
            List<AlmaResult> results = new(historyList.Count);

            // determine price weights
            double m = offset * (lookbackPeriods - 1);
            double s = lookbackPeriods / sigma;

            decimal[] weight = new decimal[lookbackPeriods];
            decimal norm = 0;

            for (int i = 0; i < lookbackPeriods; i++)
            {
                decimal wt = (decimal)Math.Exp(-((i - m) * (i - m)) / (2 * s * s));
                weight[i] = wt;
                norm += wt;
            }

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                AlmaResult r = new()
                {
                    Date = h.Date
                };

                if (index >= lookbackPeriods)
                {
                    decimal weightedSum = 0m;
                    int n = 0;

                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        TQuote d = historyList[p];
                        weightedSum += weight[n] * d.Close;
                        n++;
                    }

                    r.Alma = weightedSum / norm;
                }

                results.Add(r);
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<AlmaResult> PruneWarmupPeriods(
            this IEnumerable<AlmaResult> results)
        {
            int prunePeriods = results
                .ToList()
                .FindIndex(x => x.Alma != null);

            return results.Prune(prunePeriods);
        }


        // parameter validation
        private static void ValidateAlma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriods,
            double offset,
            double sigma)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 1 for ALMA.");
            }

            if (offset is < 0 or > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), offset,
                    "Offset must be between 0 and 1 for ALMA.");
            }

            if (sigma <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sigma), sigma,
                    "Sigma must be greater than 0 for ALMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for ALMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
