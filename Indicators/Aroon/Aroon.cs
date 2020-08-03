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
            history = Cleaners.PrepareHistory(history);

            // validate parameters
            ValidateAroon(history, lookbackPeriod);

            // initialize
            List<AroonResult> results = new List<AroonResult>();

            // roll through history
            foreach (Quote h in history)
            {

                AroonResult result = new AroonResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                // add aroons
                if (h.Index >= lookbackPeriod)
                {
                    IEnumerable<Quote> period = history
                        .Where(x => x.Index <= h.Index && x.Index > (h.Index - lookbackPeriod));

                    decimal lastHighPrice = period.Select(x => x.High).Max();
                    decimal lastLowPrice = period.Select(x => x.Low).Min();

                    int lastHighIndex = period
                        .Where(x => x.High == lastHighPrice)
                        .OrderBy(x => x.Index)  // implies "new" high, so not picking new tie for high
                        .Select(x => (int)x.Index)
                        .FirstOrDefault();

                    int lastLowIndex = period
                        .Where(x => x.Low == lastLowPrice)
                        .OrderBy(x => x.Index)
                        .Select(x => (int)x.Index)
                        .FirstOrDefault();

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
                        string.Format(cultureProvider,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }
        }
    }

}
