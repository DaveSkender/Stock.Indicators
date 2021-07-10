using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SUPERTREND
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<SuperTrendResult> GetSuperTrend<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 10,
            decimal multiplier = 3)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateSuperTrend(quotes, lookbackPeriods, multiplier);

            // initialize
            List<SuperTrendResult> results = new(historyList.Count);
            List<AtrResult> atrResults = GetAtr(quotes, lookbackPeriods).ToList();

            bool isBullish = true;
            decimal? upperBand = null;
            decimal? lowerBand = null;

            // roll through quotes
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];

                SuperTrendResult r = new()
                {
                    Date = h.Date
                };

                if (i >= lookbackPeriods - 1)
                {

                    decimal mid = (h.High + h.Low) / 2;
                    decimal atr = (decimal)atrResults[i].Atr;
                    decimal prevClose = historyList[i - 1].Close;

                    // potential bands
                    decimal upperEval = mid + multiplier * atr;
                    decimal lowerEval = mid - multiplier * atr;

                    // initial values
                    if (i == lookbackPeriods - 1)
                    {
                        isBullish = (h.Close >= mid);

                        upperBand = upperEval;
                        lowerBand = lowerEval;
                    }

                    // new upper band
                    if (upperEval < upperBand || prevClose > upperBand)
                    {
                        upperBand = upperEval;
                    }

                    // new lower band
                    if (lowerEval > lowerBand || prevClose < lowerBand)
                    {
                        lowerBand = lowerEval;
                    }

                    // supertrend
                    if (h.Close <= ((isBullish) ? lowerBand : upperBand))
                    {
                        r.SuperTrend = upperBand;
                        r.UpperBand = upperBand;
                        isBullish = false;
                    }
                    else
                    {
                        r.SuperTrend = lowerBand;
                        r.LowerBand = lowerBand;
                        isBullish = true;
                    }
                }

                results.Add(r);
            }

            return results;
        }


        // remove recommended periods extensions
        public static IEnumerable<SuperTrendResult> RemoveWarmupPeriods(
            this IEnumerable<SuperTrendResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.SuperTrend != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateSuperTrend<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            decimal multiplier)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 1 for SuperTrend.");
            }

            if (multiplier <= 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                    "Multiplier must be greater than 0 for SuperTrend.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for SuperTrend.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least N+250 data points prior to the intended "
                    + "usage date for better precision.", qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
