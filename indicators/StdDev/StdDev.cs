using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // STANDARD DEVIATION
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<StdDevResult> GetStdDev<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            int? smaPeriods = null)
            where TQuote : IQuote
        {

            // convert to basic data
            List<BasicData> bdList = quotes.ConvertToBasic("C");

            // calculate
            return CalcStdDev(bdList, lookbackPeriods, smaPeriods);
        }


        // remove recommended periods extensions
        public static IEnumerable<StdDevResult> RemoveWarmupPeriods(
            this IEnumerable<StdDevResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.StdDev != null);

            return results.Remove(removePeriods);
        }


        // internals
        private static IEnumerable<StdDevResult> CalcStdDev(
            List<BasicData> bdList, int lookbackPeriods, int? smaPeriods = null)
        {

            // check parameter arguments
            ValidateStdDev(bdList, lookbackPeriods, smaPeriods);

            // initialize
            List<StdDevResult> results = new(bdList.Count);

            // roll through quotes
            for (int i = 0; i < bdList.Count; i++)
            {
                BasicData bd = bdList[i];
                int index = i + 1;

                StdDevResult result = new()
                {
                    Date = bd.Date,
                };

                if (index >= lookbackPeriods)
                {
                    double[] periodValues = new double[lookbackPeriods];
                    decimal sum = 0m;
                    int n = 0;

                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        BasicData d = bdList[p];
                        periodValues[n] = (double)d.Value;
                        sum += d.Value;
                        n++;
                    }

                    decimal periodAvg = sum / lookbackPeriods;

                    result.StdDev = (decimal)Functions.StdDev(periodValues);
                    result.Mean = periodAvg;

                    result.ZScore = (result.StdDev == 0) ? null
                        : (bd.Value - periodAvg) / result.StdDev;
                }

                results.Add(result);

                // optional SMA
                if (smaPeriods != null && index >= lookbackPeriods + smaPeriods - 1)
                {
                    decimal sumSma = 0m;
                    for (int p = index - (int)smaPeriods; p < index; p++)
                    {
                        sumSma += (decimal)results[p].StdDev;
                    }

                    result.StdDevSma = sumSma / smaPeriods;
                }
            }

            return results;
        }


        // parameter validation
        private static void ValidateStdDev(
            List<BasicData> quotes,
            int lookbackPeriods,
            int? smaPeriods)
        {

            // check parameter arguments
            if (lookbackPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 1 for Standard Deviation.");
            }

            if (smaPeriods is not null and <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                    "SMA periods must be greater than 0 for Standard Deviation.");
            }

            // check quotes
            int qtyHistory = quotes.Count;
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Standard Deviation.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
