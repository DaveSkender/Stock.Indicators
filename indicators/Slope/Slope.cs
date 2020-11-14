using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SLOPE AND LINEAR REGRESSION
        public static IEnumerable<SlopeResult> GetSlope(IEnumerable<Quote> history, int lookbackPeriod)
        {
            // clean quotes
            List<Quote> historyList = Cleaners.PrepareHistory(history).ToList();

            // validate parameters
            ValidateSlope(history, lookbackPeriod);

            // initialize
            List<SlopeResult> results = new List<SlopeResult>();

            // roll through history for interim data
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];

                SlopeResult r = new SlopeResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                results.Add(r);

                // skip initialization period
                if (h.Index < lookbackPeriod)
                {
                    continue;
                }

                // get averages for period
                decimal sumX = 0m;
                decimal sumY = 0m;

                for (int p = r.Index - lookbackPeriod; p < r.Index; p++)
                {
                    Quote d = historyList[p];

                    sumX += (decimal)d.Index;
                    sumY += d.Close;
                }

                decimal avgX = sumX / lookbackPeriod;
                decimal avgY = sumY / lookbackPeriod;

                // least squares method
                decimal sumSqX = 0m;
                decimal sumSqY = 0m;
                decimal sumSqXY = 0m;

                for (int p = r.Index - lookbackPeriod; p < r.Index; p++)
                {
                    Quote d = historyList[p];

                    decimal devX = ((decimal)d.Index - avgX);
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
            SlopeResult last = results[historyList.Count - 1];

            for (int p = last.Index - lookbackPeriod; p < last.Index; p++)
            {
                SlopeResult d = results[p];
                d.Line = last.Slope * d.Index + last.Intercept;
            }

            return results;
        }


        private static void ValidateSlope(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // check parameters
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
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }

}
