using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // EXPONENTIAL MOVING AVERAGE
        public static IEnumerable<EmaResult> GetEma(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // initialize results
            List<EmaResult> results = new List<EmaResult>();

            // initialize EMA
            decimal k = 2 / (decimal)(lookbackPeriod + 1);
            decimal lastEma = history
                .Where(x => x.Index < lookbackPeriod)
                .Select(x => x.Close)
                .Average();

            // roll through history
            foreach (Quote h in history)
            {

                EmaResult result = new EmaResult
                {
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod - 1)
                {
                    result.Ema = lastEma + k * (h.Close - lastEma);
                    lastEma = (decimal)result.Ema;
                }

                results.Add(result);
            }

            return results;
        }

    }
}
