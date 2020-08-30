using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // Money Flow Index
        public static IEnumerable<MfiResult> GetMfi(IEnumerable<Quote> history, int lookbackPeriod = 14)
        {

            // clean quotes
            Cleaners.PrepareHistory(history);

            // check parameters
            ValidateMfi(history, lookbackPeriod);

            // initialize
            List<MfiResult> results = new List<MfiResult>();

            decimal? prevTP = null;

            // preliminary data
            foreach (Quote h in history)
            {

                MfiResult result = new MfiResult
                {
                    Index = (int)h.Index,
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
            foreach (MfiResult r in results.Where(x => x.Index > lookbackPeriod))
            {
                IEnumerable<MfiResult> period = results
                    .Where(x => x.Index > r.Index - lookbackPeriod && x.Index <= r.Index);

                IEnumerable<MfiResult> posMFs = period.Where(x => x.Direction == 1);
                IEnumerable<MfiResult> negMFs = period.Where(x => x.Direction == -1);

                // handle no negative case
                if (!negMFs.Any() || negMFs.Select(x => x.RawMF).Sum() == 0)
                {
                    r.Mfi = 100;
                    continue;
                }

                // calculate MFI normally
                decimal mfRatio = posMFs.Select(x => x.RawMF).Sum() / negMFs.Select(x => x.RawMF).Sum();

                r.Mfi = 100 - (100 / (1 + mfRatio));
            }

            return results;
        }


        private static void ValidateMfi(IEnumerable<Quote> history, int lookbackPeriod)
        {
            // check parameters
            if (lookbackPeriod <= 1)
            {
                throw new BadParameterException("Lookback period must be greater than 1 for MFI.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Money Flow Index.  " +
                        string.Format(cultureProvider,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }

        }
    }

}
