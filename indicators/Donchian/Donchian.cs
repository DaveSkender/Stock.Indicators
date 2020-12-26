using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // DONCHIAN CHANNEL
        public static IEnumerable<DonchianResult> GetDonchian<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod = 20)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateDonchian(history, lookbackPeriod);

            // initialize
            List<DonchianResult> results = new List<DonchianResult>(historyList.Count);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                DonchianResult result = new DonchianResult
                {
                    Date = h.Date
                };

                if (index >= lookbackPeriod)
                {
                    decimal highHigh = 0;
                    decimal lowLow = decimal.MaxValue;

                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        TQuote d = historyList[p];

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
                    result.Width = (result.Centerline == 0) ? null
                        : (result.UpperBand - result.LowerBand) / result.Centerline;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateDonchian<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for Donchian Channel.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Donchian Channel.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }

    }
}