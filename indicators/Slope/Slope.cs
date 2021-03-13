using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SLOPE AND LINEAR REGRESSION
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<SlopeResult> GetSlope<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateSlope(history, lookbackPeriod);

            // initialize
            int size = historyList.Count;
            List<SlopeResult> results = new(size);

            // roll through history
            for (int i = 0; i < size; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                SlopeResult r = new()
                {
                    Date = h.Date
                };

                results.Add(r);

                // skip initialization period
                if (index < lookbackPeriod)
                {
                    continue;
                }

                // get averages for period
                decimal sumX = 0m;
                decimal sumY = 0m;

                for (int p = index - lookbackPeriod; p < index; p++)
                {
                    TQuote d = historyList[p];

                    sumX += p + 1m;
                    sumY += d.Close;
                }

                decimal avgX = sumX / lookbackPeriod;
                decimal avgY = sumY / lookbackPeriod;

                // least squares method
                decimal sumSqX = 0m;
                decimal sumSqY = 0m;
                decimal sumSqXY = 0m;

                for (int p = index - lookbackPeriod; p < index; p++)
                {
                    TQuote d = historyList[p];

                    decimal devX = (p + 1m - avgX);
                    decimal devY = (d.Close - avgY);

                    sumSqX += devX * devX;
                    sumSqY += devY * devY;
                    sumSqXY += devX * devY;
                }

                r.Slope = sumSqXY / sumSqX;
                r.Intercept = avgY - r.Slope * avgX;

                // calculate Standard Deviation and R-Squared
                double stdDevX = Math.Sqrt((double)sumSqX / lookbackPeriod);
                double stdDevY = Math.Sqrt((double)sumSqY / lookbackPeriod);
                r.StdDev = stdDevY;

                if (stdDevX * stdDevY != 0)
                {
                    double R = ((double)sumSqXY / (stdDevX * stdDevY)) / lookbackPeriod;
                    r.RSquared = R * R;
                }
            }

            // add last Line (y = mx + b)
            SlopeResult last = results.LastOrDefault();
            for (int p = size - lookbackPeriod; p < size; p++)
            {
                SlopeResult d = results[p];
                d.Line = last.Slope * (p + 1) + last.Intercept;
            }

            return results;
        }


        private static void ValidateSlope<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Slope/Linear Regression.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Slope/Linear Regression.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
