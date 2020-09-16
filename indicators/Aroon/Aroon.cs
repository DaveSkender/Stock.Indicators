using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // AROON OSCILLATOR
        public static IEnumerable<AroonResult> GetAroon(IEnumerable<Quote> history, int lookbackPeriod = 25)
        {

            // clean quotes
            List<Quote> historyList = Cleaners.PrepareHistory(history).ToList();

            // validate parameters
            ValidateAroon(history, lookbackPeriod);

            // initialize
            List<AroonResult> results = new List<AroonResult>();

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];

                AroonResult result = new AroonResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                // add aroons
                if (h.Index > lookbackPeriod)
                {
                    decimal lastHighPrice = 0;
                    decimal lastLowPrice = decimal.MaxValue;
                    int lastHighIndex = 0;
                    int lastLowIndex = 0;

                    for (int p = (int)h.Index - lookbackPeriod - 1; p < h.Index; p++)
                    {
                        Quote d = historyList[p];

                        if (d.High > lastHighPrice)
                        {
                            lastHighPrice = d.High;
                            lastHighIndex = (int)d.Index;
                        }

                        if (d.Low < lastLowPrice)
                        {
                            lastLowPrice = d.Low;
                            lastLowIndex = (int)d.Index;
                        }
                    }

                    result.AroonUp = 100 * (decimal)(lookbackPeriod - (h.Index - lastHighIndex)) / lookbackPeriod;
                    result.AroonDown = 100 * (decimal)(lookbackPeriod - (h.Index - lastLowIndex)) / lookbackPeriod;
                    result.Oscillator = result.AroonUp - result.AroonDown;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateAroon(IEnumerable<Quote> history, int lookbackPeriod)
        {
            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for Aroon.");
            }

            // checked history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Aroon.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }
        }
    }

}
