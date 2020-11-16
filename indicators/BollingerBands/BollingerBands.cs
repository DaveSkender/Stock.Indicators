using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // BOLLINGER BANDS
        public static IEnumerable<BollingerBandsResult> GetBollingerBands(
            IEnumerable<Quote> history, int lookbackPeriod = 20, decimal standardDeviations = 2)
        {

            // clean quotes
            List<Quote> historyList = Cleaners.PrepareHistory(history).ToList();

            // validate parameters
            ValidateBollingerBands(history, lookbackPeriod, standardDeviations);

            // initialize
            List<BollingerBandsResult> results = new List<BollingerBandsResult>();

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];

                BollingerBandsResult r = new BollingerBandsResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod)
                {
                    double[] periodClose = new double[lookbackPeriod];
                    decimal sum = 0m;
                    int n = 0;

                    for (int p = (int)h.Index - lookbackPeriod; p < h.Index; p++)
                    {
                        Quote d = historyList[p];
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


        private static void ValidateBollingerBands(
            IEnumerable<Quote> history, int lookbackPeriod, decimal standardDeviations)
        {

            // check parameters
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
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }

    }

}
