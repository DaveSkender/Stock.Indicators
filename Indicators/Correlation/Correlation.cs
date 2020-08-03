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

            // validate parameters
            ValidateCorrelation(historyA, historyB, lookbackPeriod);

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
                r.RSquared = r.Correlation * r.Correlation;
            }

            return results;
        }


        private static void ValidateCorrelation(IEnumerable<Quote> historyA, IEnumerable<Quote> historyB, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for Correlation.");
            }

            // check history
            int qtyHistoryA = historyA.Count();
            int minHistoryA = lookbackPeriod;
            if (qtyHistoryA < minHistoryA)
            {
                throw new BadHistoryException("Insufficient history provided for Correlation.  " +
                        string.Format(cultureProvider,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistoryA, minHistoryA));
            }

            int qtyHistoryB = historyB.Count();
            if (qtyHistoryB < qtyHistoryA)
            {
                throw new BadHistoryException(
                    "B history should have at least as many records as A history for Correlation.");
            }

        }
    }

}
