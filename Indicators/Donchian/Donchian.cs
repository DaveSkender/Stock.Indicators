using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // DONCHIAN CHANNEL
        public static IEnumerable<DonchianResult> GetDonchian(
            IEnumerable<Quote> history, int lookbackPeriod = 20)
        {

            // clean quotes
            Cleaners.PrepareHistory(history);

            // validate parameters
            ValidateDonchian(history, lookbackPeriod);

            // initialize
            List<DonchianResult> results = new List<DonchianResult>();

            // roll through history
            foreach (Quote h in history)
            {
                DonchianResult result = new DonchianResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod)
                {
                    List<Quote> period = history
                        .Where(x => x.Index > (h.Index - lookbackPeriod) && x.Index <= h.Index)
                        .ToList();

                    result.UpperBand = period.Select(h => h.High).Max();
                    result.LowerBand = period.Select(l => l.Low).Min();
                    result.Centerline = (result.UpperBand + result.LowerBand) / 2;
                    result.Width = (result.Centerline == 0) ? null : (result.UpperBand - result.LowerBand) / result.Centerline;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateDonchian(
            IEnumerable<Quote> history, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 1)
            {
                throw new BadParameterException("Lookback period must be greater than 1 for Donchian Channel.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Donchian Channel.  " +
                        string.Format(cultureProvider,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }
        }

    }

}
