using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // WILLIAMS ALLIGATOR
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<AlligatorResult> GetAlligator<TQuote>(
            this IEnumerable<TQuote> quotes,
            int jawPeriods = 13,
            int jawOffset = 8,
            int teethPeriods = 8,
            int teethOffset = 5,
            int lipsPeriods = 5,
            int lipsOffset = 3)
            where TQuote : IQuote
        {

            // convert quotes
            List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.HL2);

            // check parameter arguments
            ValidateAlligator(
                quotes,
                jawPeriods,
                jawOffset,
                teethPeriods,
                teethOffset,
                lipsPeriods,
                lipsOffset);

            // initialize
            int size = bdList.Count;
            double[] pr = new double[size]; // median price

            List<AlligatorResult> results =
                bdList
                .Select(x => new AlligatorResult
                {
                    Date = x.Date
                })
                .ToList();

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                BasicD q = bdList[i];
                int index = i + 1;
                pr[i] = q.Value;

                // only calculate jaw if the array index + offset is still in valid range
                if (i + jawOffset < size)
                {
                    AlligatorResult jawResult = results[i + jawOffset];

                    // calculate alligator's jaw
                    // first value: calculate SMA
                    if (index == jawPeriods)
                    {
                        double sumMedianPrice = 0;
                        for (int p = index - jawPeriods; p < index; p++)
                        {
                            sumMedianPrice += pr[p];
                        }

                        jawResult.Jaw = (decimal)sumMedianPrice / jawPeriods;
                    }
                    // remaining values: SMMA
                    else if (index > jawPeriods)
                    {
                        double? prevValue = (double)results[i + jawOffset - 1].Jaw;
                        jawResult.Jaw = (decimal)(prevValue * (jawPeriods - 1) + pr[i]) / jawPeriods;
                    }
                }

                // only calculate teeth if the array index + offset is still in valid range
                if (i + teethOffset < size)
                {
                    AlligatorResult teethResult = results[i + teethOffset];

                    // calculate alligator's teeth
                    // first value: calculate SMA
                    if (index == teethPeriods)
                    {
                        double sumMedianPrice = 0;
                        for (int p = index - teethPeriods; p < index; p++)
                        {
                            sumMedianPrice += pr[p];
                        }

                        teethResult.Teeth = (decimal)sumMedianPrice / teethPeriods;
                    }
                    // remaining values: SMMA
                    else if (index > teethPeriods)
                    {
                        double? prevValue = (double)results[i + teethOffset - 1].Teeth;
                        teethResult.Teeth = (decimal)(prevValue * (teethPeriods - 1) + pr[i]) / teethPeriods;
                    }
                }

                // only calculate lips if the array index + offset is still in valid range
                if (i + lipsOffset < size)
                {
                    AlligatorResult lipsResult = results[i + lipsOffset];

                    // calculate alligator's lips
                    // first value: calculate SMA
                    if (index == lipsPeriods)
                    {
                        double sumMedianPrice = 0;
                        for (int p = index - lipsPeriods; p < index; p++)
                        {
                            sumMedianPrice += pr[p];
                        }

                        lipsResult.Lips = (decimal)sumMedianPrice / lipsPeriods;
                    }
                    // remaining values: SMMA
                    else if (index > lipsPeriods)
                    {
                        double? prevValue = (double)results[i + lipsOffset - 1].Lips;
                        lipsResult.Lips = (decimal)(prevValue * (lipsPeriods - 1) + pr[i]) / lipsPeriods;
                    }
                }
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<AlligatorResult> RemoveWarmupPeriods(
            this IEnumerable<AlligatorResult> results)
        {
            int removePeriods = results
              .ToList()
              .FindIndex(x => x.Jaw != null) + 251;

            return results.Remove(removePeriods);
        }


        private static void ValidateAlligator<TQuote>(
            IEnumerable<TQuote> quotes,
            int jawPeriods,
            int jawOffset,
            int teethPeriods,
            int teethOffset,
            int lipsPeriods,
            int lipsOffset)
            where TQuote : IQuote
        {
            // check parameter arguments
            if (jawPeriods <= teethPeriods)
            {
                throw new ArgumentOutOfRangeException(nameof(jawPeriods), jawPeriods,
                    "Jaw lookback periods must be greater than Teeth lookback periods for Alligator.");
            }

            if (teethPeriods <= lipsPeriods)
            {
                throw new ArgumentOutOfRangeException(nameof(teethPeriods), teethPeriods,
                    "Teeth lookback periods must be greater than Lips lookback periods for Alligator.");
            }

            if (lipsPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lipsPeriods), lipsPeriods,
                    "Lips lookback periods must be greater than 0 for Alligator.");
            }

            if (jawOffset <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(jawOffset), jawOffset,
                    "Jaw offset periods must be greater than 0 for Alligator.");
            }

            if (teethOffset <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(teethOffset), teethOffset,
                    "Jaw offset periods must be greater than 0 for Alligator.");
            }

            if (lipsOffset <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lipsOffset), lipsOffset,
                    "Jaw offset periods must be greater than 0 for Alligator.");
            }

            if (jawPeriods + jawOffset <= teethPeriods + teethOffset)
            {
                throw new ArgumentOutOfRangeException(nameof(jawPeriods), jawPeriods,
                    "Jaw lookback + offset are too small for Alligator.");
            }

            if (teethPeriods + teethOffset <= lipsPeriods + lipsOffset)
            {
                throw new ArgumentOutOfRangeException(nameof(teethPeriods), teethPeriods,
                    "Teeth lookback + offset are too small for Alligator.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = jawPeriods + jawOffset + 100;
            int recHistory = jawPeriods + jawOffset + 250;

            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Williams Alligator.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least {2} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, recHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
