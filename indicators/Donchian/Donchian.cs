using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // DONCHIAN CHANNEL
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<DonchianResult> GetDonchian<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 20)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateDonchian(quotes, lookbackPeriods);

            // initialize
            List<DonchianResult> results = new(historyList.Count);

            // roll through quotes
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];

                DonchianResult result = new()
                {
                    Date = h.Date
                };

                if (i >= lookbackPeriods)
                {
                    decimal highHigh = 0;
                    decimal lowLow = decimal.MaxValue;

                    // high/low over prior periods
                    for (int p = i - lookbackPeriods; p < i; p++)
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


        // prune recommended periods extensions
        public static IEnumerable<DonchianResult> PruneWarmupPeriods(
            this IEnumerable<DonchianResult> results)
        {
            int prunePeriods = results
              .ToList()
              .FindIndex(x => x.Width != null);

            return results.Prune(prunePeriods);
        }


        // parameter validation
        private static void ValidateDonchian<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Donchian Channel.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Donchian Channel.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(quotes), message);
            }
        }
    }
}
