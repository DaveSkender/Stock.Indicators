using System;
using System.Collections.Generic;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // CHOPPINESS INDEX
        /// <include file='./info.xml' path='indicator/*' />
        ///
        public static IEnumerable<ChopResult> GetChop<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod = 14)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateChop(historyList, lookbackPeriod);

            // initialize
            decimal sum;
            decimal high;
            decimal low;
            decimal range;

            int size = historyList.Count;
            List<ChopResult> results = new List<ChopResult>(size);
            decimal[] trueHigh = new decimal[size];
            decimal[] trueLow = new decimal[size];
            decimal[] trueRange = new decimal[size];

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                ChopResult r = new ChopResult
                {
                    Date = historyList[i].Date
                };
                results.Add(r);

                if (i > 0)
                {
                    trueHigh[i] = Math.Max(historyList[i].High, historyList[i - 1].Close);
                    trueLow[i] = Math.Min(historyList[i].Low, historyList[i - 1].Close);
                    trueRange[i] = trueHigh[i] - trueLow[i];

                    // calculate CHOP

                    if (i >= lookbackPeriod)
                    {
                        // reset measurements
                        sum = trueRange[i];
                        high = trueHigh[i];
                        low = trueLow[i];

                        // iterate over lookback window
                        for (int j = 1; j < lookbackPeriod; j++)
                        {
                            sum += trueRange[i - j];
                            high = Math.Max(high, trueHigh[i - j]);
                            low = Math.Min(low, trueLow[i - j]);
                        }

                        range = high - low;

                        // calculate CHOP
                        if (range != 0)
                        {
                            r.Chop = (decimal)(100 * (Math.Log((double)(sum / range)) / Math.Log(lookbackPeriod)));
                        }
                    }
                }
            }
            return results;
        }

        private static void ValidateChop<TQuote>(
            List<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote

        {
            // check parameter arguments
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for CHOP.");
            }

            // check history
            int qtyHistory = history.Count;
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for CHOP.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
