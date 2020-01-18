using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // STOCHASTIC RSI
        public static IEnumerable<StochRsiResult> GetStochRsi(IEnumerable<Quote> history, int lookbackPeriod = 14)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // initialize
            List<StochRsiResult> results = new List<StochRsiResult>();
            IEnumerable<RsiResult> rsiResults = GetRsi(history, lookbackPeriod);


            // calculate
            foreach (Quote h in history)
            {

                StochRsiResult result = new StochRsiResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                };

                if (h.Index >= 2 * lookbackPeriod - 1)
                {
                    IEnumerable<RsiResult> lookback = rsiResults.Where(x => x.Index <= h.Index && x.Index >= (h.Index - lookbackPeriod + 1));
                    float? rsi = lookback.Where(x => x.Index == h.Index).FirstOrDefault().Rsi;
                    float? rsiHigh = lookback.Select(x => x.Rsi).Max();
                    float? rsiLow = lookback.Select(x => x.Rsi).Min();

                    result.StochRsi = (rsi - rsiLow) / (rsiHigh - rsiLow);
                }

                results.Add(result);
            }


            // add direction
            float? lastRSI = 0;
            foreach (StochRsiResult r in results.Where(x => x.Index >= 2 * lookbackPeriod - 1).OrderBy(d => d.Index))
            {
                if (r.Index >= lookbackPeriod)
                {
                    r.IsIncreasing = (r.StochRsi >= lastRSI) ? true : false;
                }

                lastRSI = r.StochRsi;
            }

            return results;
        }

    }

}
