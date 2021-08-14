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
            this IEnumerable<TQuote> quotes,
            int smaPeriods = 20,
            decimal multiplier = 2,
            int atrPeriods = 10)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateStarcBands(quotes, smaPeriods, multiplier, atrPeriods);

            // initialize
            List<StarcBandsResult> results = new(quotesList.Count);
            List<SmaResult> smaResults = GetSma(quotes, smaPeriods).ToList();
            List<AtrResult> atrResults = GetAtr(quotes, atrPeriods).ToList();
            int lookbackPeriods = Math.Max(smaPeriods, atrPeriods);

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                StarcBandsResult result = new()
                {
                    Date = q.Date
                };

                if (index >= lookbackPeriods)
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


        // remove recommended periods
        /// <include file='../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<StarcBandsResult> RemoveWarmupPeriods(
            this IEnumerable<StarcBandsResult> results)
        {
            int n = results
                .ToList()
                .FindIndex(x => x.UpperBand != null || x.LowerBand != null) + 1;

            return results.Remove(n + 150);
        }


        // parameter validation
        private static void ValidateStarcBands<TQuote>(
            IEnumerable<TQuote> quotes,
            int smaPeriods,
            decimal multiplier,
            int atrPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (smaPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                    "EMA periods must be greater than 1 for STARC Bands.");
            }

            if (atrPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(atrPeriods), atrPeriods,
                    "ATR periods must be greater than 1 for STARC Bands.");
            }

            if (multiplier <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                    "Multiplier must be greater than 0 for STARC Bands.");
            }

            // check quotes
            int lookbackPeriods = Math.Max(smaPeriods, atrPeriods);
            int qtyHistory = quotes.Count();
            int minHistory = Math.Max(lookbackPeriods, atrPeriods + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for STARC Bands.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                    + "Since this uses a smoothing technique, for {2} lookback periods "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods, atrPeriods + 150);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
