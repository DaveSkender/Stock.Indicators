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
            this IEnumerable<TQuote> historyA,
            IEnumerable<TQuote> historyB,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> historyListA = historyA.Sort();
            List<TQuote> historyListB = historyB.Sort();

            // check parameter arguments
            ValidateCorrelation(historyA, historyB, lookbackPeriods);

            // initialize
            List<CorrResult> results = new(historyListA.Count);

            // roll through quotes
            for (int i = 0; i < historyListA.Count; i++)
            {
                TQuote a = historyListA[i];
                TQuote b = historyListB[i];
                int index = i + 1;

                if (a.Date != b.Date)
                {
                    throw new BadQuotesException(nameof(historyA), a.Date,
                        "Date sequence does not match.  Correlation requires matching dates in provided histories.");
                }

                CorrResult r = new()
                {
                    Date = a.Date
                };

                // compute correlation
                if (index >= lookbackPeriods)
                {
                    decimal sumPriceA = 0m;
                    decimal sumPriceB = 0m;
                    decimal sumPriceA2 = 0m;
                    decimal sumPriceB2 = 0m;
                    decimal sumPriceAB = 0m;

                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        TQuote qa = historyListA[p];
                        TQuote qb = historyListB[p];

                        sumPriceA += qa.Close;
                        sumPriceB += qb.Close;
                        sumPriceA2 += qa.Close * qa.Close;
                        sumPriceB2 += qb.Close * qb.Close;
                        sumPriceAB += qa.Close * qb.Close;
                    }

                    decimal avgA = sumPriceA / lookbackPeriods;
                    decimal avgB = sumPriceB / lookbackPeriods;
                    decimal avgA2 = sumPriceA2 / lookbackPeriods;
                    decimal avgB2 = sumPriceB2 / lookbackPeriods;
                    decimal avgAB = sumPriceAB / lookbackPeriods;

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


        // prune recommended periods extensions
        public static IEnumerable<CorrResult> PruneWarmupPeriods(
            this IEnumerable<CorrResult> results)
        {
            int prunePeriods = results
              .ToList()
              .FindIndex(x => x.Correlation != null);

            return results.Prune(prunePeriods);
        }


        // parameter validation
        private static void ValidateCorrelation<TQuote>(
            IEnumerable<TQuote> historyA,
            IEnumerable<TQuote> historyB,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Correlation.");
            }

            // check quotes
            int qtyHistoryA = historyA.Count();
            int minHistoryA = lookbackPeriods;
            if (qtyHistoryA < minHistoryA)
            {
                string message = "Insufficient quotes provided for Correlation.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistoryA, minHistoryA);

                throw new BadQuotesException(nameof(historyA), message);
            }

            int qtyHistoryB = historyB.Count();
            if (qtyHistoryB != qtyHistoryA)
            {
                throw new BadQuotesException(
                    nameof(historyB),
                    "B quotes should have at least as many records as A quotes for Correlation.");
            }
        }
    }
}
