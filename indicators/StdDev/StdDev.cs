using System;
using System.Collections.Generic;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // STANDARD DEVIATION
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<StdDevResult> GetStdDev<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod,
            int? smaPeriod = null)
            where TQuote : IQuote
        {

            // convert to basic data
            List<BasicData> bdList = history.ConvertToBasic("C");

            // calculate
            return CalcStdDev(bdList, lookbackPeriod, smaPeriod);
        }


        private static IEnumerable<StdDevResult> CalcStdDev(
            List<BasicData> bdList, int lookbackPeriod, int? smaPeriod = null)
        {

            // check parameter arguments
            ValidateStdDev(bdList, lookbackPeriod, smaPeriod);

            // initialize
            List<StdDevResult> results = new(bdList.Count);

            // roll through history
            for (int i = 0; i < bdList.Count; i++)
            {
                BasicData bd = bdList[i];
                int index = i + 1;

                StdDevResult result = new()
                {
                    Date = bd.Date,
                };

                if (index >= lookbackPeriod)
                {
                    double[] periodValues = new double[lookbackPeriod];
                    decimal sum = 0m;
                    int n = 0;

                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        BasicData d = bdList[p];
                        periodValues[n] = (double)d.Value;
                        sum += d.Value;
                        n++;
                    }

                    decimal periodAvg = sum / lookbackPeriod;

                    result.StdDev = (decimal)Functions.StdDev(periodValues);
                    result.Mean = periodAvg;

                    result.ZScore = (result.StdDev == 0) ? null
                        : (bd.Value - periodAvg) / result.StdDev;
                }

                results.Add(result);

                // optional SMA
                if (smaPeriod != null && index >= lookbackPeriod + smaPeriod - 1)
                {
                    decimal sumSma = 0m;
                    for (int p = index - (int)smaPeriod; p < index; p++)
                    {
                        sumSma += (decimal)results[p].StdDev;
                    }

                    result.StdDevSma = sumSma / smaPeriod;
                }
            }

            return results;
        }


        private static void ValidateStdDev(
            List<BasicData> history,
            int lookbackPeriod,
            int? smaPeriod)
        {

            // check parameter arguments
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for Standard Deviation.");
            }

            if (smaPeriod is not null and <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriod), smaPeriod,
                    "SMA period must be greater than 0 for Standard Deviation.");
            }

            // check history
            int qtyHistory = history.Count;
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Standard Deviation.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
