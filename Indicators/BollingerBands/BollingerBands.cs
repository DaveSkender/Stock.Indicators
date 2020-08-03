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
            history = Cleaners.PrepareHistory(history);

            // validate parameters
            ValidateBollingerBands(history, lookbackPeriod, standardDeviations);

            // initialize
            List<BollingerBandsResult> results = new List<BollingerBandsResult>();
            decimal? prevWidth = null;

            // roll through history
            foreach (Quote h in history)
            {
                BollingerBandsResult result = new BollingerBandsResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod)
                {
                    IEnumerable<double> periodClose = history
                        .Where(x => x.Index > (h.Index - lookbackPeriod) && x.Index <= h.Index)
                        .Select(x => (double)x.Close);

                    double stdDev = Functions.StdDev(periodClose);

                    result.Sma = (decimal)periodClose.Average();
                    result.UpperBand = result.Sma + standardDeviations * (decimal)stdDev;
                    result.LowerBand = result.Sma - standardDeviations * (decimal)stdDev;

                    result.ZScore = (stdDev == 0) ? null : (h.Close - result.Sma) / (decimal)stdDev;
                    result.Width = (result.Sma == 0) ? null : (result.UpperBand - result.LowerBand) / result.Sma;

                    if (prevWidth != null)
                    {
                        result.IsDiverging = (result.Width > prevWidth);
                    }

                    // for next iteration
                    prevWidth = result.Width;
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
                        string.Format(cultureProvider,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }
        }

    }

}
