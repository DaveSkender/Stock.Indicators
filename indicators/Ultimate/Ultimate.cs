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
            this IEnumerable<TQuote> history,
            int shortPeriod = 7,
            int middlePeriod = 14,
            int longPeriod = 28)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateUltimate(history, shortPeriod, middlePeriod, longPeriod);

            // initialize
            int size = historyList.Count;
            List<UltimateResult> results = new(size);
            decimal[] bp = new decimal[size]; // buying pressure
            decimal[] tr = new decimal[size]; // true range

            decimal priorClose = 0;

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                UltimateResult r = new()
                {
                    Date = h.Date
                };
                results.Add(r);

                if (i > 0)
                {
                    bp[i] = h.Close - Math.Min(h.Low, priorClose);
                    tr[i] = Math.Max(h.High, priorClose) - Math.Min(h.Low, priorClose);
                }

                if (index >= longPeriod + 1)
                {
                    decimal sumBP1 = 0m;
                    decimal sumBP2 = 0m;
                    decimal sumBP3 = 0m;

                    decimal sumTR1 = 0m;
                    decimal sumTR2 = 0m;
                    decimal sumTR3 = 0m;

                    for (int p = index - longPeriod; p < index; p++)
                    {
                        int pIndex = p + 1;

                        // short aggregate
                        if (pIndex > index - shortPeriod)
                        {
                            sumBP1 += bp[p];
                            sumTR1 += tr[p];
                        }

                        // middle aggregate
                        if (pIndex > index - middlePeriod)
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

                priorClose = h.Close;
            }

            return results;
        }


        private static void ValidateUltimate<TQuote>(
            IEnumerable<TQuote> history,
            int shortPeriod,
            int middleAverage,
            int longPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (shortPeriod <= 0 || middleAverage <= 0 || longPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(longPeriod), longPeriod,
                    "Average periods must be greater than 0 for Ultimate Oscillator.");
            }

            if (shortPeriod >= middleAverage || middleAverage >= longPeriod)
            {
                throw new ArgumentOutOfRangeException(nameof(middleAverage), middleAverage,
                    "Average periods must be increasingly larger than each other for Ultimate Oscillator.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = longPeriod + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Ultimate.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
