using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // AROON OSCILLATOR
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<AroonResult> GetAroon<TQuote>(
            this IEnumerable<TQuote> history,
            int lookbackPeriod = 25)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateAroon(history, lookbackPeriod);

            // initialize
            List<AroonResult> results = new(historyList.Count);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                AroonResult result = new()
                {
                    Date = h.Date
                };

                // add aroons
                if (index > lookbackPeriod)
                {
                    decimal lastHighPrice = 0;
                    decimal lastLowPrice = decimal.MaxValue;
                    int lastHighIndex = 0;
                    int lastLowIndex = 0;

                    for (int p = index - lookbackPeriod - 1; p < index; p++)
                    {
                        TQuote d = historyList[p];

                        if (d.High > lastHighPrice)
                        {
                            lastHighPrice = d.High;
                            lastHighIndex = p + 1;
                        }

                        if (d.Low < lastLowPrice)
                        {
                            lastLowPrice = d.Low;
                            lastLowIndex = p + 1;
                        }
                    }

                    result.AroonUp = 100 * (decimal)(lookbackPeriod - (index - lastHighIndex)) / lookbackPeriod;
                    result.AroonDown = 100 * (decimal)(lookbackPeriod - (index - lastLowIndex)) / lookbackPeriod;
                    result.Oscillator = result.AroonUp - result.AroonDown;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateAroon<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Aroon.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Aroon.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
