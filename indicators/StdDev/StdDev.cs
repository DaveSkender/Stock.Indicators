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
            List<BasicData> basicDataList = Cleaners.PrepareBasicData(basicData).ToList();

            // validate inputs
            ValidateStdDev(basicData, lookbackPeriod);

            // initialize results
            List<StdDevResult> results = new List<StdDevResult>();

            // roll through history and compute lookback standard deviation
            foreach (BasicData bd in basicDataList)
            {
                StdDevResult result = new StdDevResult
                {
                    Index = (int)bd.Index,
                    Date = bd.Date,
                };

                if (bd.Index >= lookbackPeriod)
                {
                    double[] periodValues = new double[lookbackPeriod];
                    decimal sum = 0m;
                    int n = 0;

                    for (int p = (int)bd.Index - lookbackPeriod; p < bd.Index; p++)
                    {
                        BasicData d = basicDataList[p];
                        periodValues[n] = (double)d.Value;
                        sum += d.Value;
                        n++;
                    }

                    decimal periodAvg = sum / lookbackPeriod;

                    result.StdDev = (decimal)Functions.StdDev(periodValues);
                    result.ZScore = (bd.Value - periodAvg) / result.StdDev;
                }

                results.Add(result);
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
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }
        }
    }



}
