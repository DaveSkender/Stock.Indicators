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
            Cleaners.PrepareHistory(history);

            // validate parameters
            ValidateBollingerBands(history, lookbackPeriod, standardDeviations);

            // initialize
            List<Quote> historyList = history.ToList();
            List<BollingerBandsResult> results = new List<BollingerBandsResult>();

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];

                BollingerBandsResult result = new BollingerBandsResult
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

                    result.Sma = periodAvg;
                    result.UpperBand = periodAvg + standardDeviations * stdDev;
                    result.LowerBand = periodAvg - standardDeviations * stdDev;

                    result.ZScore = (stdDev == 0) ? null : (h.Close - result.Sma) / stdDev;
                    result.Width = (result.Sma == 0) ? null : (result.UpperBand - result.LowerBand) / result.Sma;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateBollingerBands(
            IEnumerable<Quote> history, int lookbackPeriod, decimal standardDeviations)
        {

            // check parameters
            if (lookbackPeriod <= 1)
            {
                throw new BadParameterException("Lookback period must be greater than 1 for Bollinger Bands.");
            }

            if (standardDeviations <= 0)
            {
                throw new BadParameterException("Standard Deviations must be greater than 0 for Bollinger Bands.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Bollinger Bands.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }
        }

    }

}
