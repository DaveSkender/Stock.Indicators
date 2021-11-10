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
            this IEnumerable<TQuote> quotesA,
            IEnumerable<TQuote> quotesB,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesListA = quotesA.Sort();
            List<TQuote> quotesListB = quotesB.Sort();

            // check parameter arguments
            ValidateCorrelation(quotesA, quotesB, lookbackPeriods);

            // initialize
            List<CorrResult> results = new(quotesListA.Count);

            // roll through quotes
            for (int i = 0; i < quotesListA.Count; i++)
            {
                TQuote a = quotesListA[i];
                TQuote b = quotesListB[i];
                int index = i + 1;

                if (a.Date != b.Date)
                {
                    throw new BadQuotesException(nameof(quotesA), a.Date,
                        "Date sequence does not match.  Correlation requires matching dates in provided histories.");
                }

                CorrResult r = new()
                {
                    Date = a.Date
                };

                // calculate correlation
                if (index >= lookbackPeriods)
                {
                    decimal[] dataA = new decimal[lookbackPeriods];
                    decimal[] dataB = new decimal[lookbackPeriods];
                    int z = 0;

                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        dataA[z] = quotesListA[p].Close;
                        dataB[z] = quotesListB[p].Close;

                        z++;
                    }

                    r.CalcCorrelation(dataA, dataB);
                }

                results.Add(r);
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<CorrResult> RemoveWarmupPeriods(
            this IEnumerable<CorrResult> results)
        {
            int removePeriods = results
              .ToList()
              .FindIndex(x => x.Correlation != null);

            return results.Remove(removePeriods);
        }


        // calculate correlation
        private static void CalcCorrelation(
            this CorrResult r,
            decimal[] dataA,
            decimal[] dataB
            )
        {
            int size = dataA.Length;
            decimal sumA = 0m;
            decimal sumB = 0m;
            decimal sumA2 = 0m;
            decimal sumB2 = 0m;
            decimal sumAB = 0m;

            for (int i = 0; i < size; i++)
            {
                decimal a = dataA[i];
                decimal b = dataB[i];

                sumA += a;
                sumB += b;
                sumA2 += a * a;
                sumB2 += b * b;
                sumAB += a * b;
            }

            decimal avgA = sumA / size;
            decimal avgB = sumB / size;
            decimal avgA2 = sumA2 / size;
            decimal avgB2 = sumB2 / size;
            decimal avgAB = sumAB / size;

            r.VarianceA = avgA2 - avgA * avgA;
            r.VarianceB = avgB2 - avgB * avgB;
            r.Covariance = avgAB - avgA * avgB;

            double divisor = Math.Sqrt((double)(r.VarianceA * r.VarianceB));

            r.Correlation = (divisor == 0) ? null : r.Covariance / (decimal)divisor;

            r.RSquared = r.Correlation * r.Correlation;
        }


        // parameter validation
        private static void ValidateCorrelation<TQuote>(
            IEnumerable<TQuote> quotesA,
            IEnumerable<TQuote> quotesB,
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
            int qtyHistoryA = quotesA.Count();
            int minHistoryA = lookbackPeriods;
            if (qtyHistoryA < minHistoryA)
            {
                string message = "Insufficient quotes provided for Correlation.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistoryA, minHistoryA);

                throw new BadQuotesException(nameof(quotesA), message);
            }

            int qtyHistoryB = quotesB.Count();
            if (qtyHistoryB != qtyHistoryA)
            {
                throw new BadQuotesException(
                    nameof(quotesB),
                    "B quotes should have at least as many records as A quotes for Correlation.");
            }
        }
    }
}
