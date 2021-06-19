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
            this IEnumerable<TQuote> history)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateAlligator(history);

            // initialize
            int size = historyList.Count;
            decimal[] pr = new decimal[size]; // median price

            int jawLookback = 13;
            int jawOffset = 8;
            int teethLookback = 8;
            int teethOffset = 5;
            int lipsLookback = 5;
            int lipsOffset = 3;

            List<AlligatorResult> results =
                historyList
                .Select(x => new AlligatorResult
                {
                    Date = x.Date
                })
                .ToList();

            // roll through history
            for (int i = 0; i < size; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;
                pr[i] = (h.High + h.Low) / 2;

                // only calculate jaw if the array index + offset is still in valid range
                if (i + jawOffset < size)
                {
                    AlligatorResult jawResult = results[i + jawOffset];

                    // calculate alligator's jaw
                    // first value: calculate SMA
                    if (index == jawLookback)
                    {
                        decimal sumMedianPrice = 0m;
                        for (int p = index - jawLookback; p < index; p++)
                        {
                            sumMedianPrice += pr[p];
                        }

                        jawResult.Jaw = sumMedianPrice / jawLookback;
                    }
                    // remaining values: SMMA
                    else if (index > jawLookback)
                    {
                        decimal? prevValue = results[i + jawOffset - 1].Jaw;
                        jawResult.Jaw = (prevValue * (jawLookback - 1) + pr[i]) / jawLookback;
                    }
                }

                // only calculate teeth if the array index + offset is still in valid range
                if (i + teethOffset < size)
                {
                    AlligatorResult teethResult = results[i + teethOffset];

                    // calculate alligator's teeth
                    // first value: calculate SMA
                    if (index == teethLookback)
                    {
                        decimal sumMedianPrice = 0m;
                        for (int p = index - teethLookback; p < index; p++)
                        {
                            sumMedianPrice += pr[p];
                        }

                        teethResult.Teeth = sumMedianPrice / teethLookback;
                    }
                    // remaining values: SMMA
                    else if (index > teethLookback)
                    {
                        decimal? prevValue = results[i + teethOffset - 1].Teeth;
                        teethResult.Teeth = (prevValue * (teethLookback - 1) + pr[i]) / teethLookback;
                    }
                }

                // only calculate lips if the array index + offset is still in valid range
                if (i + lipsOffset < size)
                {
                    AlligatorResult lipsResult = results[i + lipsOffset];

                    // calculate alligator's lips
                    // first value: calculate SMA
                    if (index == lipsLookback)
                    {
                        decimal sumMedianPrice = 0m;
                        for (int p = index - lipsLookback; p < index; p++)
                        {
                            sumMedianPrice += pr[p];
                        }

                        lipsResult.Lips = sumMedianPrice / lipsLookback;
                    }
                    // remaining values: SMMA
                    else if (index > lipsLookback)
                    {
                        decimal? prevValue = results[i + lipsOffset - 1].Lips;
                        lipsResult.Lips = (prevValue * (lipsLookback - 1) + pr[i]) / lipsLookback;
                    }
                }
            }

            return results;
        }

        private static void ValidateAlligator<TQuote>(
            IEnumerable<TQuote> history)
            where TQuote : IQuote
        {
            // check history
            int qtyHistory = history.Count();

            // static values for traditional Williams Alligator with max lookback of 13
            int minHistory = 115;
            int recHistory = 265;

            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Williams Alligator.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least {2} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, recHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
