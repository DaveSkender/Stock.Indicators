using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // BOLLINGER BANDS
        public static IEnumerable<BollingerBandsResult> GetBollingerBands(IEnumerable<Quote> history, int lookbackPeriod = 20, decimal standardDeviations = 2)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check exceptions
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Bollinger Bands.  " +
                        string.Format("You provided {0} periods of history when {1} is required.", qtyHistory, minHistory));
            }

            // initialize
            List<BollingerBandsResult> results = new List<BollingerBandsResult>();
            decimal? prevUpperBand = null;
            decimal? prevLowerBand = null;

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

                    if (prevUpperBand != null && prevLowerBand != null)
                    {
                        result.IsDiverging = ((decimal)result.UpperBand - (decimal)result.LowerBand) > ((decimal)prevUpperBand - (decimal)prevLowerBand) ? true : false;
                    }

                    // for next iteration
                    prevUpperBand = result.UpperBand;
                    prevLowerBand = result.LowerBand;
                }

                results.Add(result);
            }

            return results;
        }

    }

}
