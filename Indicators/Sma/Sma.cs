using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE
        public static IEnumerable<SmaResult> GetSma(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // initialize results
            List<SmaResult> results = new List<SmaResult>();

            // roll through history
            foreach (Quote h in history)
            {

                SmaResult result = new SmaResult
                {
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod - 1)
                {
                    IEnumerable<decimal> period = history
                        .Where(x => x.Index <= h.Index && x.Index > (h.Index - lookbackPeriod))
                        .Select(x => x.Close);

                    result.Sma = period.Average();
                }

                results.Add(result);
            }

            return results;
        }

    }

}
