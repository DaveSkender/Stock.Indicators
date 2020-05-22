using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // CORRELATION COEFFICIENT
        public static IEnumerable<CorrResult> GetCorrelation(
            IEnumerable<Quote> historyA, IEnumerable<Quote> historyB, int lookbackPeriod)
        {
            // clean quotes
            historyA = Cleaners.PrepareHistory(historyA);
            historyB = Cleaners.PrepareHistory(historyB);

            // check exceptions
            int qtyHistory = historyA.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Correlation.  " +
                        string.Format("You provided {0} periods of history when {1} is required.", qtyHistory, minHistory));
            }

            // initialize
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

                r.VarianceA = avgA2 - avgA * avgA;
                r.VarianceB = avgB2 - avgB * avgB;
                r.Covariance = avgAB - avgA * avgB;
                r.Correlation = r.Covariance / (decimal)Math.Sqrt((double)(r.VarianceA * r.VarianceB));
            }

            return results;
        }

    }

}
