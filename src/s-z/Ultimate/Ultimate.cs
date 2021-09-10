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
            decimal[] bp = new decimal[size]; // buying pressure
            decimal[] tr = new decimal[size]; // true range

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
                    bp[i] = q.Close - Math.Min(q.Low, priorClose);
                    tr[i] = Math.Max(q.High, priorClose) - Math.Min(q.Low, priorClose);
                }

                if (index >= longPeriods + 1)
                {
                    decimal sumBP1 = 0m;
                    decimal sumBP2 = 0m;
                    decimal sumBP3 = 0m;

                    decimal sumTR1 = 0m;
                    decimal sumTR2 = 0m;
                    decimal sumTR3 = 0m;

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

                    decimal? avg1 = (sumTR1 == 0) ? null : sumBP1 / sumTR1;
                    decimal? avg2 = (sumTR2 == 0) ? null : sumBP2 / sumTR2;
                    decimal? avg3 = (sumTR3 == 0) ? null : sumBP3 / sumTR3;

                    r.Ultimate = 100 * (4m * avg1 + 2m * avg2 + avg3) / 7m;
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
