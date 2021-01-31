using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // CORRELATION COEFFICIENT
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<CorrResult> GetCorrelation<TQuote>(
            IEnumerable<TQuote> historyA,
            IEnumerable<TQuote> historyB,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyListA = historyA.Sort();
            List<TQuote> historyListB = historyB.Sort();

            // check parameter arguments
            ValidateCorrelation(historyA, historyB, lookbackPeriod);

            // initialize
            List<CorrResult> results = new List<CorrResult>(historyListA.Count);
            decimal sumPriceA = 0m;
            decimal sumPriceB = 0m;
            decimal sumPriceA2 = 0m;
            decimal sumPriceB2 = 0m;
            decimal sumPriceAB = 0m;

            // roll through history
            for (int i = 0; i < historyListA.Count; i++)
            {
                int index = i + 1;
                TQuote a = historyListA[i];
                TQuote b = historyListB[i];

                if (a.Date != b.Date)
                {
                    throw new BadHistoryException(nameof(historyA), a.Date,
                        "Date sequence does not match.  Correlation requires matching dates in provided histories.");
                }

                CorrResult r = new CorrResult
                {
                    Date = a.Date
                };

                sumPriceA += a.Close;
                sumPriceB += b.Close;
                sumPriceA2 += a.Close * a.Close;
                sumPriceB2 += b.Close * b.Close;
                sumPriceAB += a.Close * b.Close;

                // compute correlation
                if (index >= lookbackPeriod)
                {

                    if (index != lookbackPeriod)
                    {
                        TQuote qa = historyListA[i - lookbackPeriod];
                        TQuote qb = historyListB[i - lookbackPeriod];

                        sumPriceA -= qa.Close;
                        sumPriceB -= qb.Close;
                        sumPriceA2 -= qa.Close * qa.Close;
                        sumPriceB2 -= qb.Close * qb.Close;
                        sumPriceAB -= qa.Close * qb.Close;
                    }

                    decimal avgA = sumPriceA / lookbackPeriod;
                    decimal avgB = sumPriceB / lookbackPeriod;
                    decimal avgA2 = sumPriceA2 / lookbackPeriod;
                    decimal avgB2 = sumPriceB2 / lookbackPeriod;
                    decimal avgAB = sumPriceAB / lookbackPeriod;

                    r.VarianceA = avgA2 - avgA * avgA;
                    r.VarianceB = avgB2 - avgB * avgB;
                    r.Covariance = avgAB - avgA * avgB;

                    double divisor = Math.Sqrt((double)(r.VarianceA * r.VarianceB));

                    r.Correlation = (divisor == 0) ? null : r.Covariance / (decimal)divisor;

                    r.RSquared = r.Correlation * r.Correlation;
                }

                results.Add(r);
            }

            return results;
        }


        private static void ValidateCorrelation<TQuote>(
            IEnumerable<TQuote> historyA,
            IEnumerable<TQuote> historyB,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Correlation.");
            }

            // check history
            int qtyHistoryA = historyA.Count();
            int minHistoryA = lookbackPeriod;
            if (qtyHistoryA < minHistoryA)
            {
                string message = "Insufficient history provided for Correlation.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistoryA, minHistoryA);

                throw new BadHistoryException(nameof(historyA), message);
            }

            int qtyHistoryB = historyB.Count();
            if (qtyHistoryB != qtyHistoryA)
            {
                throw new BadHistoryException(
                    nameof(historyB),
                    "B history should have at least as many records as A history for Correlation.");
            }
        }
    }
}
