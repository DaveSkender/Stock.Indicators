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

            // initialize
            List<BollingerBandsResult> results = new List<BollingerBandsResult>();
            IEnumerable<SmaResult> sma = GetSma(history, lookbackPeriod);
            decimal? prevUpperBand = null;
            decimal? prevLowerBand = null;

            // roll through history
            foreach (Quote h in history)
            {
                BollingerBandsResult result = new BollingerBandsResult
                {
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod - 1)
                {
                    IEnumerable<double> periodClose = history
                        .Where(x => x.Index > (h.Index - lookbackPeriod) && x.Index <= h.Index)
                        .Select(x => (double)x.Close);

                    double stdDev = Functions.StdDev(periodClose);

                    result.Sma = sma.Where(x => x.Date == h.Date).Select(x => x.Sma).FirstOrDefault();
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
