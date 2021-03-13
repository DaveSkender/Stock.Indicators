using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // MONEY FLOW INDEX
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<MfiResult> GetMfi<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod = 14)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateMfi(history, lookbackPeriod);

            // initialize
            int size = historyList.Count;
            List<MfiResult> results = new(size);
            decimal[] tp = new decimal[size];  // true price
            decimal[] mf = new decimal[size];  // raw MF value
            int[] direction = new int[size];   // direction

            decimal? prevTP = null;

            // roll through history, to get preliminary data
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];

                MfiResult result = new()
                {
                    Date = h.Date
                };

                // true price
                tp[i] = (h.High + h.Low + h.Close) / 3;

                // raw money flow
                mf[i] = tp[i] * h.Volume;

                // direction
                if (prevTP == null || tp[i] == prevTP)
                {
                    direction[i] = 0;
                }
                else if (tp[i] > prevTP)
                {
                    direction[i] = 1;
                }
                else if (tp[i] < prevTP)
                {
                    direction[i] = -1;
                }

                results.Add(result);

                prevTP = tp[i];
            }

            // add money flow index
            for (int i = lookbackPeriod; i < results.Count; i++)
            {
                MfiResult r = results[i];
                int index = i + 1;

                decimal sumPosMFs = 0;
                decimal sumNegMFs = 0;

                for (int p = index - lookbackPeriod; p < index; p++)
                {
                    if (direction[p] == 1)
                    {
                        sumPosMFs += mf[p];
                    }
                    else if (direction[p] == -1)
                    {
                        sumNegMFs += mf[p];
                    }
                }

                // handle no negative case
                if (sumNegMFs == 0)
                {
                    r.Mfi = 100;
                    continue;
                }

                // calculate MFI normally
                decimal mfRatio = sumPosMFs / sumNegMFs;

                r.Mfi = 100 - (100 / (1 + mfRatio));
            }

            return results;
        }


        private static void ValidateMfi<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for MFI.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Money Flow Index.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
