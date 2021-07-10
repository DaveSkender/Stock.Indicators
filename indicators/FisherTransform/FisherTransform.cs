using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // FISHER TRANSFORM
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<FisherTransformResult> GetFisherTransform<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 10)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateFisherTransform(quotes, lookbackPeriods);

            // initialize
            int size = historyList.Count;
            decimal[] pr = new decimal[size]; // median price
            double[] xv = new double[size];  // price transform "value"
            List<FisherTransformResult> results = new(size);


            // roll through quotes
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                pr[i] = (h.High + h.Low) / 2m;

                decimal minPrice = pr[i];
                decimal maxPrice = pr[i];

                for (int p = Math.Max(i - lookbackPeriods + 1, 0); p <= i; p++)
                {
                    minPrice = Math.Min(pr[p], minPrice);
                    maxPrice = Math.Max(pr[p], maxPrice);
                }

                FisherTransformResult r = new()
                {
                    Date = h.Date
                };

                if (i > 0)
                {
                    xv[i] = maxPrice != minPrice
                        ? 0.33 * 2 * ((double)((pr[i] - minPrice) / (maxPrice - minPrice) - 0.5m))
                              + 0.67 * xv[i - 1]
                        : 0;

                    xv[i] = (xv[i] > 0.99) ? 0.999 : xv[i];
                    xv[i] = (xv[i] < -0.99) ? -0.999 : xv[i];

                    r.Fisher = 0.5m * (decimal)Math.Log((1 + xv[i]) / (1 - xv[i]))
                          + 0.5m * results[i - 1].Fisher;

                    r.Trigger = results[i - 1].Fisher;
                }
                else
                {
                    xv[i] = 0;
                    r.Fisher = 0;
                }

                results.Add(r);
            }

            return results;
        }


        // parameter validation
        private static void ValidateFisherTransform<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Fisher Transform.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Fisher Transform.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for {2} lookback periods "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods, lookbackPeriods + 15);

                throw new BadHistoryException(nameof(quotes), message);
            }
        }
    }
}
