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
            List<BasicD> quotesList =
                quotes.ConvertToBasic(CandlePart.Close);

            // check parameter arguments
            ValidateHurst(quotes, lookbackPeriods);

            // initialize
            int size = quotesList.Count;
            List<HurstResult> results = new(size);

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                int index = i + 1;
                BasicD q = quotesList[i];

                HurstResult result = new()
                {
                    Date = q.Date
                };

                if (index > lookbackPeriods)
                {
                    // get evaluation batch
                    double[] values = new double[lookbackPeriods];

                    int x = 0;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        // compile return values
                        if (quotesList[p - 1].Value != 0)
                        {
                            values[x] = quotesList[p].Value / quotesList[p - 1].Value - 1;
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

        private static double CalcHurst(double[] values)
        {
            int totalSize = values.Length;
            int maxChunks = 0;
            int setQty = 0;

            // determine max chunk quantity so chunks are
            // not smaller than 8 observations
            // and not to exceed 32 total chunks
            for (int chunkQty = 1; chunkQty <= 32; chunkQty *= 2)
            {
                if (totalSize / chunkQty >= 8)
                {
                    maxChunks = chunkQty;
                    setQty++;
                    continue;
                }
                else
                {
                    break;
                }
            }

            // initialize result sets
            double[] logRs = new double[setQty];
            double[] logSize = new double[setQty];
            int setNum = 0;

            // roll through sets
            for (int chunkQty = 1; chunkQty <= maxChunks; chunkQty *= 2)
            {
                // initialize set and chunks
                int chunkSize = totalSize / chunkQty;
                double? sumChunkRs = 0;

                // starting index position used to skip
                // observations to enforce same-sized chunks
                int index = totalSize - chunkSize * chunkQty;

                // analyze chunks in set
                for (int chunkNum = 1; chunkNum <= chunkQty; chunkNum++)
                {
                    // chunk mean
                    double sum = 0;
                    for (int i = index; i < index + chunkSize; i++)
                    {
                        sum += values[i];
                    }
                    double chunkMean = sum / chunkSize;

                    // chunk mean diff
                    double sumY = 0;
                    double sumSq = 0;
                    double maxY = values[index] - chunkMean;
                    double minY = values[index] - chunkMean;
                    for (int i = index; i < index + chunkSize; i++)
                    {
                        double y = values[i] - chunkMean;
                        sumY += y;
                        minY = (sumY < minY) ? sumY : minY;
                        maxY = (sumY > maxY) ? sumY : maxY;

                        sumSq += y * y;
                    }

                    // chunk rescaled range
                    double r = maxY - minY;
                    double s = Math.Sqrt(sumSq / chunkSize);
                    double? rs = s != 0 ? r / s : null;

                    sumChunkRs += rs;

                    // increment starting index
                    index += chunkSize;
                }

                // set results
                if (sumChunkRs != null)
                {
                    logSize[setNum] = Math.Log10(chunkSize);
                    logRs[setNum] = Math.Log10((double)sumChunkRs / chunkQty);
                }

                // increment set
                setNum++;
            }

            // hurst exponent
            // TODO: apply Anis-Lloyd corrected R/S Hurst?
            return Functions.Slope(logSize, logRs);
        }


        // convert to quotes
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Convert"]/*' />
        /// 
        public static IEnumerable<Quote> ConvertToQuotes(
            this IEnumerable<HurstResult> results)
        {
            return results
              .Where(x => x.HurstExponent != null)
              .Select(x => new Quote
              {
                  Date = x.Date,
                  Open = (decimal)x.HurstExponent,
                  High = (decimal)x.HurstExponent,
                  Low = (decimal)x.HurstExponent,
                  Close = (decimal)x.HurstExponent
              })
              .ToList();
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
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
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
