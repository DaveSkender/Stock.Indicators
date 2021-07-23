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
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 9,
            double offset = 0.85,
            double sigma = 6)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateAlma(quotes, lookbackPeriods, offset, sigma);

            // initialize
            List<AlmaResult> results = new(quotesList.Count);

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

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                AlmaResult r = new()
                {
                    Date = q.Date
                };

                if (index >= lookbackPeriods)
                {
                    decimal weightedSum = 0m;
                    int n = 0;

                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        TQuote d = quotesList[p];
                        weightedSum += weight[n] * d.Close;
                        n++;
                    }

                    r.Alma = weightedSum / norm;
                }

                results.Add(r);
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<AlmaResult> RemoveWarmupPeriods(
            this IEnumerable<AlmaResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Alma != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateAlma<TQuote>(
            IEnumerable<TQuote> quotes,
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

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for ALMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
