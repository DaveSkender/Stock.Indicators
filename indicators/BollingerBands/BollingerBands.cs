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
            this IEnumerable<TQuote> history,
            int lookbackPeriod = 20,
            decimal standardDeviations = 2)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateBollingerBands(history, lookbackPeriod, standardDeviations);

            // initialize
            List<BollingerBandsResult> results = new(historyList.Count);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                BollingerBandsResult r = new()
                {
                    Date = h.Date
                };

                if (index >= lookbackPeriod)
                {
                    double[] periodClose = new double[lookbackPeriod];
                    decimal sum = 0m;
                    int n = 0;

                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        TQuote d = historyList[p];
                        periodClose[n] = (double)d.Close;
                        sum += d.Close;
                        n++;
                    }

                    decimal periodAvg = sum / lookbackPeriod;
                    decimal stdDev = (decimal)Functions.StdDev(periodClose);

                    r.Sma = periodAvg;
                    r.UpperBand = periodAvg + standardDeviations * stdDev;
                    r.LowerBand = periodAvg - standardDeviations * stdDev;

                    r.PercentB = (r.UpperBand == r.LowerBand) ? null
                        : (h.Close - r.LowerBand) / (r.UpperBand - r.LowerBand);

                    r.ZScore = (stdDev == 0) ? null : (h.Close - r.Sma) / stdDev;
                    r.Width = (r.Sma == 0) ? null : (r.UpperBand - r.LowerBand) / r.Sma;
                }

                results.Add(r);
            }

            return results;
        }


        private static void ValidateBollingerBands<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod,
            decimal standardDeviations)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for Bollinger Bands.");
            }

            if (standardDeviations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(standardDeviations), standardDeviations,
                    "Standard Deviations must be greater than 0 for Bollinger Bands.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Bollinger Bands.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
