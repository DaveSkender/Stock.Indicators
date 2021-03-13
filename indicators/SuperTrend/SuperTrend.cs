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
            IEnumerable<TQuote> history,
            int lookbackPeriod = 10,
            decimal multiplier = 3)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateSuperTrend(history, lookbackPeriod, multiplier);

            // initialize
            List<SuperTrendResult> results = new(historyList.Count);
            List<AtrResult> atrResults = GetAtr(history, lookbackPeriod).ToList();

            bool isBullish = true;
            decimal? upperBand = null;
            decimal? lowerBand = null;

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];

                SuperTrendResult r = new()
                {
                    Date = h.Date
                };

                if (i >= lookbackPeriod - 1)
                {

                    decimal mid = (h.High + h.Low) / 2;
                    decimal atr = (decimal)atrResults[i].Atr;
                    decimal prevClose = historyList[i - 1].Close;

                    // potential bands
                    decimal upperEval = mid + multiplier * atr;
                    decimal lowerEval = mid - multiplier * atr;

                    // initial values
                    if (i == lookbackPeriod - 1)
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


        private static void ValidateSuperTrend<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod,
            decimal multiplier)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for SuperTrend.");
            }

            if (multiplier <= 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                    "Multiplier must be greater than 0 for SuperTrend.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for SuperTrend.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least N+250 data points prior to the intended "
                    + "usage date for better precision.", qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
