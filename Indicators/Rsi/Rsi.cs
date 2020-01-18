using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RELATIVE STRENGTH INDEX
        public static IEnumerable<RsiResult> GetRsi(IEnumerable<Quote> history, int lookbackPeriod = 14)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // initialize
            decimal lastClose = history.First().Close;
            List<RsiResult> results = new List<RsiResult>();

            // load gain data
            foreach (Quote h in history)
            {

                RsiResult result = new RsiResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                    Gain = (lastClose < h.Close) ? (float)(h.Close - lastClose) : 0,
                    Loss = (lastClose > h.Close) ? (float)(lastClose - h.Close) : 0
                };
                results.Add(result);

                lastClose = h.Close;
            }

            // initialize average gain
            float avgGain = results.Where(x => x.Index <= lookbackPeriod).Select(g => g.Gain).Average();
            float avgLoss = results.Where(x => x.Index <= lookbackPeriod).Select(g => g.Loss).Average();

            // initial RSI for trend analysis
            float lastRSI = (avgLoss > 0) ? 100 - (100 / (1 + (avgGain / avgLoss))) : 100;


            // calculate RSI
            foreach (RsiResult r in results.Where(x => x.Index >= lookbackPeriod).OrderBy(d => d.Index))
            {
                avgGain = (avgGain * (lookbackPeriod - 1) + r.Gain) / lookbackPeriod;
                avgLoss = (avgLoss * (lookbackPeriod - 1) + r.Loss) / lookbackPeriod;

                if (avgLoss > 0)
                {
                    float rs = avgGain / avgLoss;
                    r.Rsi = 100 - (100 / (1 + rs));
                }
                else
                {
                    r.Rsi = 100;
                }

                r.IsIncreasing = (r.Rsi >= lastRSI) ? true : false;
                lastRSI = (float)r.Rsi;
            }

            return results;
        }

    }

}
