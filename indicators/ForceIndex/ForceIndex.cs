using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // FORCE INDEX
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<ForceIndexResult> GetForceIndex<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateForceIndex(history, lookbackPeriod);

            // initialize
            int size = historyList.Count;
            List<ForceIndexResult> results = new List<ForceIndexResult>(size);
            decimal? prevClose = null, prevFI = null;
            decimal k = 2m / (lookbackPeriod + 1), sumRawFI = 0m;

            // roll through history
            for (int i = 0; i < size; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                ForceIndexResult r = new ForceIndexResult
                {
                    Date = h.Date
                };
                results.Add(r);

                // skip first period
                if (i == 0)
                {
                    prevClose = h.Close;
                    continue;
                }

                // raw Force Index
                decimal? rawFI = h.Volume * (h.Close - prevClose);
                prevClose = h.Close;

                // calculate EMA
                if (index > lookbackPeriod + 1)
                {
                    r.ForceIndex = prevFI + k * (rawFI - prevFI);
                }

                // initialization period
                else
                {
                    sumRawFI += (decimal)rawFI;

                    // first EMA value
                    if (index == lookbackPeriod + 1)
                    {
                        r.ForceIndex = sumRawFI / lookbackPeriod;
                    }
                }

                prevFI = r.ForceIndex;
            }

            return results;
        }


        private static void ValidateForceIndex<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Force Index.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(2 * lookbackPeriod, lookbackPeriod + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Force Index.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for a lookback period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriod, lookbackPeriod + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
