using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // STANDARD DEVIATION
        public static IEnumerable<StdDevResult> GetStdDev(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // convert to basic data
            IEnumerable<BasicData> bd = Cleaners.ConvertHistoryToBasic(history, "C");

            // calculate
            return CalcStdDev(bd, lookbackPeriod);
        }


        private static IEnumerable<StdDevResult> CalcStdDev(IEnumerable<BasicData> basicData, int lookbackPeriod)
        {
            // clean data
            basicData = Cleaners.PrepareBasicData(basicData);

            // validate inputs
            ValidateStdDev(basicData, lookbackPeriod);

            // initialize results
            List<StdDevResult> results = new List<StdDevResult>();
            decimal? prevValue = null;

            // roll through history and compute lookback standard deviation
            foreach (BasicData h in basicData)
            {
                StdDevResult result = new StdDevResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                };

                if (h.Index >= lookbackPeriod)
                {
                    // price based
                    double[] period = basicData
                        .Where(x => x.Index > (h.Index - lookbackPeriod) && x.Index <= h.Index)
                        .Select(x => (double)x.Value)
                        .ToArray();

                    result.StdDev = (decimal)Functions.StdDev(period);
                    result.ZScore = (h.Value - (decimal)period.Average()) / result.StdDev;
                }

                results.Add(result);
                prevValue = h.Value;
            }

            return results;
        }


        private static void ValidateStdDev(IEnumerable<BasicData> basicData, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 1)
            {
                throw new BadParameterException("Lookback period must be greater than 1 for Standard Deviation.");
            }

            // check history
            int qtyHistory = basicData.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Standard Deviation.  " +
                        string.Format(cultureProvider,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }
        }
    }



}
