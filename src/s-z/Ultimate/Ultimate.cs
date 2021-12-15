using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ULTIMATE OSCILLATOR
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<UltimateResult> GetUltimate<TQuote>(
            this IEnumerable<TQuote> quotes,
            int shortPeriods = 7,
            int middlePeriods = 14,
            int longPeriods = 28)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateUltimate(quotes, shortPeriods, middlePeriods, longPeriods);

            // initialize
            int size = quotesList.Count;
            List<UltimateResult> results = new(size);
            double[] bp = new double[size]; // buying pressure
            double[] tr = new double[size]; // true range

            decimal priorClose = 0;

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                UltimateResult r = new()
                {
                    Date = q.Date
                };
                results.Add(r);

                if (i > 0)
                {
                    bp[i] = (double)(q.Close - Math.Min(q.Low, priorClose));
                    tr[i] = (double)(Math.Max(q.High, priorClose) - Math.Min(q.Low, priorClose));
                }

                if (index >= longPeriods + 1)
                {
                    double sumBP1 = 0;
                    double sumBP2 = 0;
                    double sumBP3 = 0;

                    double sumTR1 = 0;
                    double sumTR2 = 0;
                    double sumTR3 = 0;

                    for (int p = index - longPeriods; p < index; p++)
                    {
                        int pIndex = p + 1;

                        // short aggregate
                        if (pIndex > index - shortPeriods)
                        {
                            sumBP1 += bp[p];
                            sumTR1 += tr[p];
                        }

                        // middle aggregate
                        if (pIndex > index - middlePeriods)
                        {
                            sumBP2 += bp[p];
                            sumTR2 += tr[p];
                        }

                        // long aggregate
                        sumBP3 += bp[p];
                        sumTR3 += tr[p];
                    }

                    double? avg1 = (sumTR1 == 0) ? null : sumBP1 / sumTR1;
                    double? avg2 = (sumTR2 == 0) ? null : sumBP2 / sumTR2;
                    double? avg3 = (sumTR3 == 0) ? null : sumBP3 / sumTR3;

                    r.Ultimate = (decimal?)(100 * (4d * avg1 + 2d * avg2 + avg3) / 7d);
                }

                priorClose = q.Close;
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<UltimateResult> RemoveWarmupPeriods(
            this IEnumerable<UltimateResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Ultimate != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateUltimate<TQuote>(
            IEnumerable<TQuote> quotes,
            int shortPeriods,
            int middleAverage,
            int longPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (shortPeriods <= 0 || middleAverage <= 0 || longPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(longPeriods), longPeriods,
                    "Average periods must be greater than 0 for Ultimate Oscillator.");
            }

            if (shortPeriods >= middleAverage || middleAverage >= longPeriods)
            {
                throw new ArgumentOutOfRangeException(nameof(middleAverage), middleAverage,
                    "Average periods must be increasingly larger than each other for Ultimate Oscillator.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = longPeriods + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Ultimate.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
