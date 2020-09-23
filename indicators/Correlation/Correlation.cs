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
            List<Quote> historyListA = Cleaners.PrepareHistory(historyA).ToList();
            List<Quote> historyListB = Cleaners.PrepareHistory(historyB).ToList();

            // validate parameters
            ValidateCorrelation(historyA, historyB, lookbackPeriod);

            // initialize
            List<CorrResult> results = new List<CorrResult>();


            // roll through history for interim data
            for (int i = 0; i < historyListA.Count; i++)
            {
                Quote a = historyListA[i];
                Quote b = historyListB[i];

                if (a.Date != b.Date)
                {
                    throw new BadHistoryException(
                        "Date sequence does not match.  Correlation requires matching dates in provided histories.");
                }

                CorrResult r = new CorrResult
                {
                    Index = (int)a.Index,
                    Date = a.Date
                };

                // compute correlation
                if (i + 1 >= lookbackPeriod)
                {
                    decimal sumPriceA = 0m;
                    decimal sumPriceB = 0m;
                    decimal sumPriceA2 = 0m;
                    decimal sumPriceB2 = 0m;
                    decimal sumPriceAB = 0m;

                    for (int p = r.Index - lookbackPeriod; p < r.Index; p++)
                    {
                        Quote qa = historyListA[p];
                        Quote qb = historyListB[p];

                        sumPriceA += qa.Close;
                        sumPriceB += qb.Close;
                        sumPriceA2 += qa.Close * qa.Close;
                        sumPriceB2 += qb.Close * qb.Close;
                        sumPriceAB += qa.Close * qb.Close;
                    }

                    decimal avgA = sumPriceA / lookbackPeriod;
                    decimal avgB = sumPriceB / lookbackPeriod;
                    decimal avgA2 = sumPriceA2 / lookbackPeriod;
                    decimal avgB2 = sumPriceB2 / lookbackPeriod;
                    decimal avgAB = sumPriceAB / lookbackPeriod;

                    r.VarianceA = avgA2 - avgA * avgA;
                    r.VarianceB = avgB2 - avgB * avgB;
                    r.Covariance = avgAB - avgA * avgB;
                    r.Correlation = r.Covariance / (decimal)Math.Sqrt((double)(r.VarianceA * r.VarianceB));
                    r.RSquared = r.Correlation * r.Correlation;
                }

                results.Add(r);
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
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistoryA, minHistoryA));
            }

            int qtyHistoryB = historyB.Count();
            if (qtyHistoryB != qtyHistoryA)
            {
                throw new BadHistoryException(
                    "B history should have at least as many records as A history for Correlation.");
            }
        }
    }

}
