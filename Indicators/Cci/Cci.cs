using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // COMMODITY CHANNEL INDEX
        public static IEnumerable<CciResult> GetCci(IEnumerable<Quote> history, int lookbackPeriod = 20)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // initialize results
            List<CciResult> results = new List<CciResult>();


            // roll through history to get Typical Price
            foreach (Quote h in history)
            {

                CciResult result = new CciResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                    Tp = (h.High + h.Low + h.Close) / 3
                };
                results.Add(result);
            }


            // roll through interim results to calculate CCI
            foreach (CciResult result in results.Where(x => x.Index >= lookbackPeriod))
            {
                IEnumerable<CciResult> period = results.Where(x => x.Index <= result.Index && x.Index > (result.Index - lookbackPeriod));

                decimal smaTp = (decimal)period.Select(x => x.Tp).Average();
                decimal meanDv = 0;

                foreach (CciResult p in period)
                {
                    meanDv += Math.Abs(smaTp - (decimal)p.Tp);
                }
                meanDv /= lookbackPeriod;
                result.Cci = (result.Tp - smaTp) / ((decimal)0.015 * meanDv);
            }

            return results;
        }

    }
}
