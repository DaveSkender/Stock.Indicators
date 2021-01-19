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
            IEnumerable<TQuote> history,
            int fastPeriod = 5,
            int slowPeriod = 34)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateAwesome(history, fastPeriod, slowPeriod);

            // initialize
            int size = historyList.Count;
            List<AwesomeResult> results = new List<AwesomeResult>();
            decimal[] pr = new decimal[size]; // median price

            // roll through history
            for (int i = 0; i < size; i++)
            {
                TQuote h = historyList[i];
                pr[i] = (h.High + h.Low) / 2;
                int index = i + 1;

                AwesomeResult r = new AwesomeResult
                {
                    Date = h.Date
                };

                if (index >= slowPeriod)
                {
                    decimal sumSlow = 0m;
                    decimal sumFast = 0m;

                    for (int p = index - slowPeriod; p < index; p++)
                    {
                        sumSlow += pr[p];

                        if (p >= index - fastPeriod)
                        {
                            sumFast += pr[p];
                        }
                    }

                    r.Oscillator = (sumFast / fastPeriod) - (sumSlow / slowPeriod);
                    r.Normalized = (pr[i] != 0) ? 100 * r.Oscillator / pr[i] : null;
                }

                results.Add(r);
            }

            return results;
        }


        private static void ValidateAwesome<TQuote>(
            IEnumerable<TQuote> history,
            int fastPeriod,
            int slowPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (fastPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriod), slowPeriod,
                    "Fast period must be greater than 0 for Awesome Oscillator.");
            }

            if (slowPeriod <= fastPeriod)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriod), slowPeriod,
                    "Slow period must be larger than Fast Period for Awesome Oscillator.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = slowPeriod;
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
