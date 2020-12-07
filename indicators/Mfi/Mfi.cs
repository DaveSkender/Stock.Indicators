using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // Money Flow Index
        public static IEnumerable<MfiResult> GetMfi<TQuote>(
            IEnumerable<TQuote> history, 
            int lookbackPeriod = 14) 
            where TQuote : IQuote
        {

            // clean quotes
            List<TQuote> historyList = history.Sort();

            // check parameters
            ValidateMfi(history, lookbackPeriod);

            // initialize
            List<MfiResult> results = new List<MfiResult>(historyList.Count);

            decimal? prevTP = null;

            // preliminary data
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];

                MfiResult result = new MfiResult
                {
                    Date = h.Date,
                    TruePrice = (h.High + h.Low + h.Close) / 3
                };

                // raw money flow
                result.RawMF = result.TruePrice * h.Volume;

                // direction
                if (prevTP == null || result.TruePrice == prevTP)
                {
                    result.Direction = 0;
                }
                else if (result.TruePrice > prevTP)
                {
                    result.Direction = 1;
                }
                else if (result.TruePrice < prevTP)
                {
                    result.Direction = -1;
                }

                results.Add(result);

                prevTP = result.TruePrice;
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
                    MfiResult d = results[p];

                    if (d.Direction == 1)
                    {
                        sumPosMFs += d.RawMF;
                    }
                    else if (d.Direction == -1)
                    {
                        sumNegMFs += d.RawMF;
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


        private static void ValidateMfi<TQuote>(IEnumerable<TQuote> history, int lookbackPeriod) where TQuote : IQuote
        {
            // check parameters
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
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }

        }
    }

}
