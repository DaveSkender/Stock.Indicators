﻿using System;
using System.Collections.Generic;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // STANDARD DEVIATION
        public static IEnumerable<StdDevResult> GetStdDev<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod,
            int? smaPeriod = null)
            where TQuote : IQuote
        {

            // convert to basic data
            List<BasicData> bd = Cleaners.ConvertHistoryToBasic(history, "C");

            // calculate
            return CalcStdDev(bd, lookbackPeriod, smaPeriod);
        }


        private static IEnumerable<StdDevResult> CalcStdDev(
            List<BasicData> bdList, int lookbackPeriod, int? smaPeriod = null)
        {

            // validate inputs
            ValidateStdDev(bdList, lookbackPeriod, smaPeriod);

            // initialize results
            List<StdDevResult> results = new List<StdDevResult>(bdList.Count);

            // roll through history and compute lookback standard deviation
            for (int i = 0; i < bdList.Count; i++)
            {
                BasicData bd = bdList[i];
                int index = i + 1;

                StdDevResult result = new StdDevResult
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

                    result.Sma = sumSma / smaPeriod;
                }
            }

            return results;
        }


        private static void ValidateStdDev(List<BasicData> history, int lookbackPeriod, int? smaPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for Standard Deviation.");
            }

            if (smaPeriod != null && smaPeriod <= 0)
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
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }

        }
    }



}
