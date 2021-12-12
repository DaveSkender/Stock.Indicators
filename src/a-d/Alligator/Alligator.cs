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
            this IEnumerable<TQuote> quotes)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateAlligator(quotes);

            // initialize
            int size = quotesList.Count;
            double[] pr = new double[size]; // median price

            int jawLookback = 13;
            int jawOffset = 8;
            int teethLookback = 8;
            int teethOffset = 5;
            int lipsLookback = 5;
            int lipsOffset = 3;

            List<AlligatorResult> results =
                quotesList
                .Select(x => new AlligatorResult
                {
                    Date = x.Date
                })
                .ToList();

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;
                pr[i] = (double)(q.High + q.Low) / 2;

                // only calculate jaw if the array index + offset is still in valid range
                if (i + jawOffset < size)
                {
                    AlligatorResult jawResult = results[i + jawOffset];

                    // calculate alligator's jaw
                    // first value: calculate SMA
                    if (index == jawLookback)
                    {
                        double sumMedianPrice = 0;
                        for (int p = index - jawLookback; p < index; p++)
                        {
                            sumMedianPrice += pr[p];
                        }

                        jawResult.Jaw = (decimal)sumMedianPrice / jawLookback;
                    }
                    // remaining values: SMMA
                    else if (index > jawLookback)
                    {
                        double? prevValue = (double)results[i + jawOffset - 1].Jaw;
                        jawResult.Jaw = (decimal)(prevValue * (jawLookback - 1) + pr[i]) / jawLookback;
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
                        double sumMedianPrice = 0;
                        for (int p = index - teethLookback; p < index; p++)
                        {
                            sumMedianPrice += pr[p];
                        }

                        teethResult.Teeth = (decimal)sumMedianPrice / teethLookback;
                    }
                    // remaining values: SMMA
                    else if (index > teethLookback)
                    {
                        double? prevValue = (double)results[i + teethOffset - 1].Teeth;
                        teethResult.Teeth = (decimal)(prevValue * (teethLookback - 1) + pr[i]) / teethLookback;
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
                        double sumMedianPrice = 0;
                        for (int p = index - lipsLookback; p < index; p++)
                        {
                            sumMedianPrice += pr[p];
                        }

                        lipsResult.Lips = (decimal)sumMedianPrice / lipsLookback;
                    }
                    // remaining values: SMMA
                    else if (index > lipsLookback)
                    {
                        double? prevValue = (double)results[i + lipsOffset - 1].Lips;
                        lipsResult.Lips = (decimal)(prevValue * (lipsLookback - 1) + pr[i]) / lipsLookback;
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
            int removePeriods = 265;
            return results.Remove(removePeriods);
        }


        private static void ValidateAlligator<TQuote>(
            IEnumerable<TQuote> quotes)
            where TQuote : IQuote
        {
            // check quotes
            int qtyHistory = quotes.Count();

            // static values for traditional Williams Alligator with max lookback of 13
            int minHistory = 115;
            int recHistory = 265;

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
