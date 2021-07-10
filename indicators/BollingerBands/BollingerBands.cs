using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // BOLLINGER BANDS
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<BollingerBandsResult> GetBollingerBands<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 20,
            decimal standardDeviations = 2)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateBollingerBands(quotes, lookbackPeriods, standardDeviations);

            // initialize
            List<BollingerBandsResult> results = new(quotesList.Count);

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                BollingerBandsResult r = new()
                {
                    Date = q.Date
                };

                if (index >= lookbackPeriods)
                {
                    double[] periodClose = new double[lookbackPeriods];
                    decimal sum = 0m;
                    int n = 0;

                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        TQuote d = quotesList[p];
                        periodClose[n] = (double)d.Close;
                        sum += d.Close;
                        n++;
                    }

                    decimal periodAvg = sum / lookbackPeriods;
                    decimal stdDev = (decimal)Functions.StdDev(periodClose);

                    r.Sma = periodAvg;
                    r.UpperBand = periodAvg + standardDeviations * stdDev;
                    r.LowerBand = periodAvg - standardDeviations * stdDev;

                    r.PercentB = (r.UpperBand == r.LowerBand) ? null
                        : (q.Close - r.LowerBand) / (r.UpperBand - r.LowerBand);

                    r.ZScore = (stdDev == 0) ? null : (q.Close - r.Sma) / stdDev;
                    r.Width = (r.Sma == 0) ? null : (r.UpperBand - r.LowerBand) / r.Sma;
                }

                results.Add(r);
            }

            return results;
        }


        // remove recommended periods extensions
        public static IEnumerable<BollingerBandsResult> RemoveWarmupPeriods(
            this IEnumerable<BollingerBandsResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Width != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateBollingerBands<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            decimal standardDeviations)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 1 for Bollinger Bands.");
            }

            if (standardDeviations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(standardDeviations), standardDeviations,
                    "Standard Deviations must be greater than 0 for Bollinger Bands.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Bollinger Bands.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
