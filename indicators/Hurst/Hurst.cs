using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // HURST EXPONENT
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<HurstResult> GetHurst<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 100)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateHurst(quotes, lookbackPeriods);

            // initialize
            int size = quotesList.Count;
            List<HurstResult> results = new(size);


            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                int index = i + 1;
                TQuote q = quotesList[i];

                HurstResult result = new()
                {
                    Date = q.Date
                };

                if (index > lookbackPeriods)
                {
                    // get evaluation batch
                    decimal[] values = new decimal[lookbackPeriods];

                    int x = 0;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        // compile return values
                        if (quotesList[p - 1].Close != 0)
                        {
                            values[x] = quotesList[p].Close / quotesList[p - 1].Close - 1;
                        }

                        x++;
                    }

                    // calculate hurst exponent
                    result.HurstExponent = CalcHurst(values);
                }
                results.Add(result);
            }

            return results;
        }

        private static decimal CalcHurst(decimal[] values)
        {
            int setNum = 1;
            int totalSize = values.Length;
            double[] logRs = new double[6];
            double[] logSize = new double[6];

            // roll through sets
            for (int n = 1; n <= 32; n *= 2)
            {
                // initialize sets
                int nominalChunkSize = totalSize / n;
                logSize[setNum - 1] = Math.Log10(nominalChunkSize);
                bool uneven = (nominalChunkSize * n != totalSize);
                int index = 0;
                decimal? sumChunkRs = 0;

                // initialize chunk
                decimal[] mean = new decimal[n];
                decimal?[] rsChunk = new decimal?[n];

                // analyze chunks in set
                for (int chunkNum = 1; chunkNum <= n; chunkNum++)
                {
                    int chunkSize = (chunkNum == 1 && uneven) ?
                          nominalChunkSize + 1 : nominalChunkSize;

                    // mean
                    decimal sum = 0m;
                    for (int i = index; i < index + chunkSize; i++)
                    {
                        sum += values[i];
                    }
                    decimal chunkMean = sum / chunkSize;
                    mean[chunkNum - 1] = chunkMean;

                    // mean diff
                    decimal sumY = 0m;
                    decimal sumSq = 0m;
                    decimal maxY = 0m;
                    decimal minY = 0m;
                    for (int i = index; i < index + chunkSize; i++)
                    {
                        decimal y = values[i] - chunkMean;
                        sumY += y;
                        sumSq += y * y;
                        minY = (sumY < minY) ? sumY : minY;
                        maxY = (sumY > maxY) ? sumY : maxY;
                    }

                    // TODO: unsure of cumulative sum of deviations method (sumY)

                    // rescaled range for chunk
                    decimal r = maxY - minY;
                    decimal s = (decimal)Math.Sqrt((double)sumSq / chunkSize);
                    decimal? rs = s != 0 ? r / s : null;
                    rsChunk[chunkNum - 1] = rs;
                    sumChunkRs += rs;

                    // increment starting index
                    index += chunkSize;
                }

                // log(average chunk rs)
                if (sumChunkRs != null)
                {
                    logRs[setNum - 1] = Math.Log10((double)sumChunkRs / n);
                }

                // increment set
                setNum++;
            }

            // TODO: see if all double arrays is any faster/less accurate

            // hurst exponent
            decimal h = (decimal)Functions.Slope(logSize, logRs);
            return h;
        }


        // remove recommended periods
        /// <include file='../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<HurstResult> RemoveWarmupPeriods(
            this IEnumerable<HurstResult> results)
        {
            int removePeriods = results
              .ToList()
              .FindIndex(x => x.HurstExponent != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateHurst<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods < 100)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be at least 100 for Hurst Exponent.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Hurst Exponent.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
