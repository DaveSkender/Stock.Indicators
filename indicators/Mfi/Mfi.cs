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
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 14)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateMfi(quotes, lookbackPeriods);

            // initialize
            int size = historyList.Count;
            List<MfiResult> results = new(size);
            decimal[] tp = new decimal[size];  // true price
            decimal[] mf = new decimal[size];  // raw MF value
            int[] direction = new int[size];   // direction

            decimal? prevTP = null;

            // roll through quotes, to get preliminary data
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
            for (int i = lookbackPeriods; i < results.Count; i++)
            {
                MfiResult r = results[i];
                int index = i + 1;

                decimal sumPosMFs = 0;
                decimal sumNegMFs = 0;

                for (int p = index - lookbackPeriods; p < index; p++)
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


        // prune recommended periods extensions
        public static IEnumerable<MfiResult> PruneWarmupPeriods(
            this IEnumerable<MfiResult> results)
        {
            int prunePeriods = results
                .ToList()
                .FindIndex(x => x.Mfi != null);

            return results.Prune(prunePeriods);
        }


        // parameter validation
        private static void ValidateMfi<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 1 for MFI.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Money Flow Index.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(quotes), message);
            }
        }
    }
}
