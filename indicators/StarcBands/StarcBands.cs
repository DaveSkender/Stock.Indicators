using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // STARC BANDS
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<StarcBandsResult> GetStarcBands<TQuote>(
            this IEnumerable<TQuote> history,
            int smaPeriod = 20,
            decimal multiplier = 2,
            int atrPeriod = 10)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateStarcBands(history, smaPeriod, multiplier, atrPeriod);

            // initialize
            List<StarcBandsResult> results = new(historyList.Count);
            List<SmaResult> smaResults = GetSma(history, smaPeriod).ToList();
            List<AtrResult> atrResults = GetAtr(history, atrPeriod).ToList();
            int lookbackPeriod = Math.Max(smaPeriod, atrPeriod);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                StarcBandsResult result = new()
                {
                    Date = h.Date
                };

                if (index >= lookbackPeriod)
                {
                    SmaResult s = smaResults[i];
                    AtrResult a = atrResults[i];

                    result.Centerline = s.Sma;
                    result.UpperBand = s.Sma + multiplier * a.Atr;
                    result.LowerBand = s.Sma - multiplier * a.Atr;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateStarcBands<TQuote>(
            IEnumerable<TQuote> history,
            int smaPeriod,
            decimal multiplier,
            int atrPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (smaPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriod), smaPeriod,
                    "EMA period must be greater than 1 for STARC Bands.");
            }

            if (atrPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(atrPeriod), atrPeriod,
                    "ATR period must be greater than 1 for STARC Bands.");
            }

            if (multiplier <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                    "Multiplier must be greater than 0 for STARC Bands.");
            }

            // check history
            int lookbackPeriod = Math.Max(smaPeriod, atrPeriod);
            int qtyHistory = history.Count();
            int minHistory = Math.Max(lookbackPeriod, atrPeriod + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for STARC Bands.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for a lookback period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriod, atrPeriod + 150);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
