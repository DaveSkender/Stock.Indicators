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
            List<Quote> historyList = Cleaners.PrepareHistory(history).ToList();

            // validate parameters
            ValidateDonchian(history, lookbackPeriod);

            // initialize
            List<DonchianResult> results = new List<DonchianResult>();

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];

                DonchianResult result = new DonchianResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod)
                {
                    decimal highHigh = 0;
                    decimal lowLow = decimal.MaxValue;

                    for (int p = (int)h.Index - lookbackPeriod; p < h.Index; p++)
                    {
                        Quote d = historyList[p];

                        if (d.High > highHigh)
                        {
                            highHigh = d.High;
                        }

                        if (d.Low < lowLow)
                        {
                            lowLow = d.Low;
                        }
                    }

                    result.UpperBand = highHigh;
                    result.LowerBand = lowLow;
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
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }
        }

    }

}
