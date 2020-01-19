using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // CORRELATION (e.g. BETA when used with Indices)
        public static IEnumerable<CorrResult> GetCorrelation(IEnumerable<Quote> historyA, IEnumerable<Quote> historyB, int lookbackPeriod)
        {
            // clean quotes
            historyA = Cleaners.PrepareHistory(historyA);
            historyB = Cleaners.PrepareHistory(historyB);

            // initialize results
            List<CorrResult> results = new List<CorrResult>();


            // roll through history for interim data
            foreach (Quote a in historyA)
            {
                Quote b = historyB.Where(x => x.Date == a.Date).FirstOrDefault();
                if (b == null)
                {
                    throw new BadHistoryException("Correlation requires matching dates in provided histories.  {0} not found in historyB.");
                }

                CorrResult result = new CorrResult
                {
                    Index = (int)a.Index,
                    Date = a.Date,
                    PriceA = a.Close,
                    PriceB = b.Close
                    // other values calculated in class properties
                };
                results.Add(result);
            }

            // compute correlation
            foreach (CorrResult r in results.Where(x => x.Index >= lookbackPeriod))
            {
                IEnumerable<CorrResult> period = results.Where(x => x.Index > (r.Index - lookbackPeriod) && x.Index <= r.Index);

                decimal avgA = period.Select(x => x.PriceA).Average();
                decimal avgB = period.Select(x => x.PriceB).Average();
                decimal avgA2 = period.Select(x => x.PriceA2).Average();
                decimal avgB2 = period.Select(x => x.PriceB2).Average();
                decimal avgAB = period.Select(x => x.PriceAB).Average();

                decimal varianceA = avgA2 - avgA * avgA;
                decimal varianceB = avgB2 - avgB * avgB;
                decimal covariance = avgAB - avgA * avgB;

                r.Correlation = covariance / (decimal)Math.Sqrt((double)(varianceA * varianceB));
            }

            return results;
        }

    }

}
