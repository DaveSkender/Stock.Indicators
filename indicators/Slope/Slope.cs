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
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateSlope(quotes, lookbackPeriods);

            // initialize
            int size = quotesList.Count;
            List<SlopeResult> results = new(size);

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                SlopeResult r = new()
                {
                    Date = q.Date
                };

                results.Add(r);

                // skip initialization period
                if (index < lookbackPeriods)
                {
                    continue;
                }

                // get averages for period
                decimal sumX = 0m;
                decimal sumY = 0m;

                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    TQuote d = quotesList[p];

                    sumX += p + 1m;
                    sumY += d.Close;
                }

                decimal avgX = sumX / lookbackPeriods;
                decimal avgY = sumY / lookbackPeriods;

                // least squares method
                decimal sumSqX = 0m;
                decimal sumSqY = 0m;
                decimal sumSqXY = 0m;

                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    TQuote d = quotesList[p];

                    decimal devX = (p + 1m - avgX);
                    decimal devY = (d.Close - avgY);

                    sumSqX += devX * devX;
                    sumSqY += devY * devY;
                    sumSqXY += devX * devY;
                }

                r.Slope = sumSqXY / sumSqX;
                r.Intercept = avgY - r.Slope * avgX;

                // calculate Standard Deviation and R-Squared
                double stdDevX = Math.Sqrt((double)sumSqX / lookbackPeriods);
                double stdDevY = Math.Sqrt((double)sumSqY / lookbackPeriods);
                r.StdDev = stdDevY;

                if (stdDevX * stdDevY != 0)
                {
                    double R = ((double)sumSqXY / (stdDevX * stdDevY)) / lookbackPeriods;
                    r.RSquared = R * R;
                }
            }

            // add last Line (y = mx + b)
            SlopeResult last = results.LastOrDefault();
            for (int p = size - lookbackPeriods; p < size; p++)
            {
                SlopeResult d = results[p];
                d.Line = last.Slope * (p + 1) + last.Intercept;
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<SlopeResult> RemoveWarmupPeriods(
            this IEnumerable<SlopeResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Slope != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateSlope<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Slope/Linear Regression.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Slope/Linear Regression.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
