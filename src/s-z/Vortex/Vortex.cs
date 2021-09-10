using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // VORTEX INDICATOR
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<VortexResult> GetVortex<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateVortex(quotes, lookbackPeriods);

            // initialize
            int size = quotesList.Count;
            List<VortexResult> results = new(size);

            decimal[] tr = new decimal[size];
            decimal[] pvm = new decimal[size];
            decimal[] nvm = new decimal[size];

            decimal prevHigh = 0;
            decimal prevLow = 0;
            decimal prevClose = 0;

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                VortexResult result = new()
                {
                    Date = q.Date
                };

                // skip first period
                if (index == 1)
                {
                    results.Add(result);
                    prevHigh = q.High;
                    prevLow = q.Low;
                    prevClose = q.Close;
                    continue;
                }

                // trend information
                decimal highMinusPrevClose = Math.Abs(q.High - prevClose);
                decimal lowMinusPrevClose = Math.Abs(q.Low - prevClose);

                tr[i] = Math.Max((q.High - q.Low), Math.Max(highMinusPrevClose, lowMinusPrevClose));
                pvm[i] = Math.Abs(q.High - prevLow);
                nvm[i] = Math.Abs(q.Low - prevHigh);

                prevHigh = q.High;
                prevLow = q.Low;
                prevClose = q.Close;

                // vortex indicator
                if (index > lookbackPeriods)
                {

                    decimal sumTr = 0;
                    decimal sumPvm = 0;
                    decimal sumNvm = 0;

                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        sumTr += tr[p];
                        sumPvm += pvm[p];
                        sumNvm += nvm[p];
                    }

                    if (sumTr is not 0)
                    {
                        result.Pvi = sumPvm / sumTr;
                        result.Nvi = sumNvm / sumTr;
                    }
                }

                results.Add(result);
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<VortexResult> RemoveWarmupPeriods(
            this IEnumerable<VortexResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Pvi != null || x.Nvi != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateVortex<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 1 for VI.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for VI.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
