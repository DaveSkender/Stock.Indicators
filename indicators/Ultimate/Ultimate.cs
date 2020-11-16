using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ULTIMATE OSCILLATOR
        public static IEnumerable<UltimateResult> GetUltimate(
            IEnumerable<Quote> history, int shortPeriod = 7, int middlePeriod = 14, int longPeriod = 28)
        {

            // clean quotes
            List<Quote> historyList = Cleaners.PrepareHistory(history).ToList();

            // check parameters
            ValidateUltimate(history, shortPeriod, middlePeriod, longPeriod);

            // initialize
            List<UltimateResult> results = new List<UltimateResult>();
            decimal priorClose = 0;

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];
                int index = i + 1;

                UltimateResult r = new UltimateResult
                {
                    Date = h.Date
                };
                results.Add(r);

                if (i > 0)
                {
                    r.Bp = h.Close - Math.Min(h.Low, priorClose);
                    r.Tr = Math.Max(h.High, priorClose) - Math.Min(h.Low, priorClose);
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
                        UltimateResult pr = results[p];
                        int pIndex = p + 1;

                        // short aggregate
                        if (pIndex > index - shortPeriod)
                        {
                            sumBP1 += (decimal)pr.Bp;
                            sumTR1 += (decimal)pr.Tr;
                        }

                        // middle aggregate
                        if (pIndex > index - middlePeriod)
                        {
                            sumBP2 += (decimal)pr.Bp;
                            sumTR2 += (decimal)pr.Tr;
                        }

                        // long aggregate
                        sumBP3 += (decimal)pr.Bp;
                        sumTR3 += (decimal)pr.Tr;
                    }

                    decimal avg1 = sumBP1 / sumTR1;
                    decimal avg2 = sumBP2 / sumTR2;
                    decimal avg3 = sumBP3 / sumTR3;

                    r.Ultimate = 100 * (4m * avg1 + 2m * avg2 + avg3) / 7m;
                }

                priorClose = h.Close;
            }

            return results;
        }


        private static void ValidateUltimate(
            IEnumerable<Quote> history, int shortPeriod = 7, int middleAverage = 14, int longPeriod = 28)
        {

            // check parameters
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
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }

        }
    }

}
