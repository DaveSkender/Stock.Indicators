using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // DONCHIAN CHANNEL
        public static IEnumerable<KeltnerResult> GetKeltner(
            IEnumerable<Quote> history, int emaPeriod = 20, decimal multiplier = 2, int atrPeriod = 10)
        {

            // clean quotes
            Cleaners.PrepareHistory(history);

            // validate parameters
            ValidateKeltner(history, emaPeriod, multiplier, atrPeriod);

            // initialize
            List<KeltnerResult> results = new List<KeltnerResult>();
            IEnumerable<EmaResult> emaResults = GetEma(history, emaPeriod);
            IEnumerable<AtrResult> atrResults = GetAtr(history, atrPeriod);
            int lookbackPeriod = Math.Max(emaPeriod, atrPeriod);

            decimal? prevWidth = null;

            // roll through history
            foreach (Quote h in history)
            {
                KeltnerResult result = new KeltnerResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod)
                {
                    IEnumerable<Quote> period = history
                        .Where(x => x.Index > (h.Index - lookbackPeriod) && x.Index <= h.Index);

                    EmaResult ema = emaResults.Where(x => x.Index == h.Index).FirstOrDefault();
                    AtrResult atr = atrResults.Where(x => x.Index == h.Index).FirstOrDefault();

                    result.UpperBand = ema.Ema + multiplier * atr.Atr;
                    result.LowerBand = ema.Ema - multiplier * atr.Atr;
                    result.Centerline = ema.Ema;
                    result.Width = (result.Centerline == 0) ? null : (result.UpperBand - result.LowerBand) / result.Centerline;

                    // for next iteration
                    prevWidth = result.Width;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateKeltner(
            IEnumerable<Quote> history, int emaPeriod, decimal multiplier, int atrPeriod)
        {

            // check parameters
            if (emaPeriod <= 1)
            {
                throw new BadParameterException("EMA period must be greater than 1 for Keltner Channel.");
            }

            if (atrPeriod <= 1)
            {
                throw new BadParameterException("ATR period must be greater than 1 for Keltner Channel.");
            }

            if (multiplier <= 0)
            {
                throw new BadParameterException("Multiplier must be greater than 0 for Keltner Channel.");
            }

            // check history
            int lookbackPeriod = Math.Max(emaPeriod, atrPeriod);
            int qtyHistory = history.Count();
            int minHistory = Math.Max(2 * lookbackPeriod, lookbackPeriod + 100);
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Keltner Channel.  " +
                        string.Format(cultureProvider,
                        "You provided {0} periods of history when at least {1} is required.  "
                          + "Since this uses a smoothing technique, for a lookback period of {2}, "
                          + "we recommend you use at least {3} data points prior to the intended "
                          + "usage date for maximum precision.",
                          qtyHistory, minHistory, lookbackPeriod, lookbackPeriod + 250));
            }
        }

    }

}
