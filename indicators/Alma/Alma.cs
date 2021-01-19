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
            IEnumerable<TQuote> history,
            int lookbackPeriod = 9,
            double offset = 0.85,
            double sigma = 6)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateAlma(history, lookbackPeriod, offset, sigma);

            // initialize
            List<AlmaResult> results = new List<AlmaResult>(historyList.Count);

            // determine price weights
            double m = offset * (lookbackPeriod - 1);
            double s = lookbackPeriod / sigma;

            decimal[] weight = new decimal[lookbackPeriod];
            decimal norm = 0;

            for (int i = 0; i < lookbackPeriod; i++)
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

                AlmaResult r = new AlmaResult
                {
                    Date = h.Date
                };

                if (index >= lookbackPeriod)
                {
                    decimal weightedSum = 0m;
                    int n = 0;

                    for (int p = index - lookbackPeriod; p < index; p++)
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


        private static void ValidateAlma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod,
            double offset,
            double sigma)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for ALMA.");
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
            int minHistory = lookbackPeriod;
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
