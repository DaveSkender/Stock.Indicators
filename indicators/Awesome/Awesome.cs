using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // AWESOME OSCILLATOR
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<AwesomeResult> GetAwesome<TQuote>(
            this IEnumerable<TQuote> history,
            int fastPeriods = 5,
            int slowPeriods = 34)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateAwesome(history, fastPeriods, slowPeriods);

            // initialize
            int size = historyList.Count;
            List<AwesomeResult> results = new();
            decimal[] pr = new decimal[size]; // median price

            // roll through history
            for (int i = 0; i < size; i++)
            {
                TQuote h = historyList[i];
                pr[i] = (h.High + h.Low) / 2;
                int index = i + 1;

                AwesomeResult r = new()
                {
                    Date = h.Date
                };

                if (index >= slowPeriods)
                {
                    decimal sumSlow = 0m;
                    decimal sumFast = 0m;

                    for (int p = index - slowPeriods; p < index; p++)
                    {
                        sumSlow += pr[p];

                        if (p >= index - fastPeriods)
                        {
                            sumFast += pr[p];
                        }
                    }

                    r.Oscillator = (sumFast / fastPeriods) - (sumSlow / slowPeriods);
                    r.Normalized = (pr[i] != 0) ? 100 * r.Oscillator / pr[i] : null;
                }

                results.Add(r);
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<AwesomeResult> PruneWarmupPeriods(
            this IEnumerable<AwesomeResult> results)
        {
            int prunePeriods = results
                .ToList()
                .FindIndex(x => x.Oscillator != null);

            return results.Prune(prunePeriods);
        }


        // parameter validation
        private static void ValidateAwesome<TQuote>(
            IEnumerable<TQuote> history,
            int fastPeriods,
            int slowPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (fastPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                    "Fast periods must be greater than 0 for Awesome Oscillator.");
            }

            if (slowPeriods <= fastPeriods)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                    "Slow periods must be larger than Fast Periods for Awesome Oscillator.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = slowPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Awesome Oscillator.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
