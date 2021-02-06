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
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateVortex(history, lookbackPeriod);

            // initialize
            int size = historyList.Count;
            List<VortexResult> results = new List<VortexResult>(size);

            decimal[] tr = new decimal[size];
            decimal[] pvm = new decimal[size];
            decimal[] nvm = new decimal[size];

            decimal prevHigh = 0;
            decimal prevLow = 0;
            decimal prevClose = 0;

            // roll through history
            for (int i = 0; i < size; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                VortexResult result = new VortexResult
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
                if (index > lookbackPeriod)
                {

                    decimal sumTr = 0;
                    decimal sumPvm = 0;
                    decimal sumNvm = 0;

                    for (int p = index - lookbackPeriod; p < index; p++)
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


        private static void ValidateVortex<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for VI.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for VI.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
