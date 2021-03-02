using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        /// <include file='./info.xml' path='indicators/type[@name="Default"]/*' />
        ///
        public static IEnumerable<AlligatorResult> GetAlligator<TQuote>(
            IEnumerable<TQuote> history
            )
            where TQuote : IQuote
        {
            return GetAlligator(history, 13, 8, 8, 5, 5, 3);
        }

        /// <include file='./info.xml' path='indicators/type[@name="Main"]/*' />
        ///
        public static IEnumerable<AlligatorResult> GetAlligator<TQuote>(
        IEnumerable<TQuote> history,
        int lookbackJaw,
        int smoothingJaw,
        int lookbackTeeth,
        int smoothingTeeth,
        int lookbackLips,
        int smoothingLips)
        where TQuote : IQuote
        {
            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateAlligator(history, lookbackJaw, smoothingJaw, lookbackTeeth, smoothingTeeth, lookbackLips, smoothingLips);

            // initialize
            List<AlligatorResult> results = new List<AlligatorResult>(historyList.Count);
            decimal? prevValue;

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                AlligatorResult result = new AlligatorResult
                {
                    Date = h.Date
                };

                // calculate alligator's jaw
                // first value: calculate SMA
                if (index == lookbackJaw)
                {
                    decimal sumClose = 0m;
                    for (int p = index - lookbackJaw; p < index; p++)
                    {
                        TQuote d = historyList[p];
                        sumClose += d.Close;
                    }

                    result.Jaw = sumClose / lookbackJaw;
                }
                // remaining values: modified SMMA
                else if (index > lookbackJaw)
                {
                    prevValue = results[i - 1].Jaw;
                    result.Jaw = ((prevValue * smoothingJaw) - prevValue + h.Close) / smoothingJaw;
                }

                // calculate alligator's teeth
                // first value: calculate SMA
                if (index == lookbackTeeth)
                {
                    decimal sumClose = 0m;
                    for (int p = index - lookbackTeeth; p < index; p++)
                    {
                        TQuote d = historyList[p];
                        sumClose += d.Close;
                    }

                    result.Teeth = sumClose / lookbackTeeth;
                }
                // remaining values: modified SMMA
                else if (index > lookbackTeeth)
                {
                    prevValue = results[i - 1].Teeth;
                    result.Teeth = ((prevValue * smoothingTeeth) - prevValue + h.Close) / smoothingTeeth;
                }

                // calculate alligator's lips
                // first value: calculate SMA
                if (index == lookbackLips)
                {
                    decimal sumClose = 0m;
                    for (int p = index - lookbackLips; p < index; p++)
                    {
                        TQuote d = historyList[p];
                        sumClose += d.Close;
                    }

                    result.Lips = sumClose / lookbackLips;
                }
                // remaining values: modified SMMA
                else if (index > lookbackLips)
                {
                    prevValue = results[i - 1].Lips;
                    result.Lips = ((prevValue * smoothingLips) - prevValue + h.Close) / smoothingLips;
                }

                results.Add(result);
            }

            return results;
        }

        private static void ValidateAlligator<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackJaw,
            int smoothingJaw,
            int lookbackTeeth,
            int smoothingTeeth,
            int lookbackLips,
            int smoothingLips)
            where TQuote : IQuote
        {
            // check parameter arguments
            if (lookbackJaw <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackJaw), lookbackJaw,
                    "Lookback period must be greater than 0 for Alligator Jaw.");
            }

            if (lookbackTeeth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackTeeth), lookbackTeeth,
                    "Lookback period must be greater than 0 for Alligator Teeth.");
            }

            if (lookbackLips <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackLips), lookbackLips,
                    "Lookback period must be greater than 0 for Alligator Lips.");
            }

            if (smoothingJaw <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothingJaw), smoothingJaw,
                    "Smoothing period must be greater than 0 for Alligator Jaw.");
            }

            if (smoothingTeeth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothingTeeth), smoothingTeeth,
                    "Smoothing period must be greater than 0 for Alligator Teeth.");
            }

            if (smoothingLips <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothingLips), smoothingLips,
                    "Smoothing period must be greater than 0 for Alligator Lips.");
            }

            if (smoothingJaw > lookbackJaw)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothingJaw), smoothingJaw,
                    "Smoothing period must be less than or equal to lookback period for Alligator Jaw.");
            }

            if (smoothingTeeth > lookbackTeeth)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothingTeeth), smoothingTeeth,
                    "Smoothing period must be less than or equal to lookback period for Alligator Teeth.");
            }

            if (smoothingLips > lookbackLips)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothingLips), smoothingLips,
                    "Smoothing period must be less than or equal to lookback period for Alligator Lips.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistoryJaw = Math.Max(2 * lookbackJaw, lookbackJaw + 100);
            int minHistoryTeeth = Math.Max(2 * lookbackTeeth, lookbackTeeth + 100);
            int minHistoryLips = Math.Max(2 * lookbackLips, lookbackLips + 100);
            int minHistory = Math.Max(Math.Max(minHistoryJaw, minHistoryTeeth), minHistoryLips);
            int minLookbackPeriod = Math.Max(Math.Max(lookbackJaw, lookbackTeeth), lookbackLips);

            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Alligator.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for a lookback period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, minLookbackPeriod, minLookbackPeriod + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
