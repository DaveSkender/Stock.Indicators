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
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateVortex(quotes, lookbackPeriods);

            // initialize
            int size = historyList.Count;
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
                TQuote h = historyList[i];
                int index = i + 1;

                VortexResult result = new()
                {
                    Date = h.Date
                };

                // skip first period
                if (index == 1)
                {
                    results.Add(result);
                    prevHigh = h.High;
                    prevLow = h.Low;
                    prevClose = h.Close;
                    continue;
                }

                // trend information
                decimal highMinusPrevClose = Math.Abs(h.High - prevClose);
                decimal lowMinusPrevClose = Math.Abs(h.Low - prevClose);

                tr[i] = Math.Max((h.High - h.Low), Math.Max(highMinusPrevClose, lowMinusPrevClose));
                pvm[i] = Math.Abs(h.High - prevLow);
                nvm[i] = Math.Abs(h.Low - prevHigh);

                prevHigh = h.High;
                prevLow = h.Low;
                prevClose = h.Close;

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


        // prune recommended periods extensions
        public static IEnumerable<VortexResult> PruneWarmupPeriods(
            this IEnumerable<VortexResult> results)
        {
            int prunePeriods = results
                .ToList()
                .FindIndex(x => x.Pvi != null || x.Nvi != null);

            return results.Prune(prunePeriods);
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
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(quotes), message);
            }
        }
    }
}
