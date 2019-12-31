using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ULCER INDEX (UI)
        public static IEnumerable<UlcerIndexResult> GetUlcerIndex(IEnumerable<Quote> history, int lookbackPeriod = 14)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // initialize
            List<UlcerIndexResult> results = new List<UlcerIndexResult>();

            // preliminary data
            foreach (Quote h in history)
            {

                UlcerIndexResult result = new UlcerIndexResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod - 1)
                {
                    IEnumerable<Quote> period = history
                        .Where(x => x.Index > (h.Index - lookbackPeriod) && x.Index <= h.Index);

                    double sumSquared = 0;

                    foreach (Quote p in period)
                    {
                        decimal maxClose = period
                            .Where(x => x.Index <= p.Index)
                            .Select(x => x.Close)
                            .Max();

                        decimal percentDrawdown = 100 * (p.Close - maxClose) / maxClose;

                        sumSquared += (double)(percentDrawdown * percentDrawdown);
                    }

                    result.UI = (decimal)Math.Sqrt(sumSquared/lookbackPeriod);
                }
                results.Add(result);
            }


            return results;
        }

    }

}
