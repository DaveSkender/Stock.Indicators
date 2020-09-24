using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // TRIPLE EXPONENTIAL MOVING AVERAGE
        public static IEnumerable<EmaResult> GetTripleEma(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // convert history to basic format
            IEnumerable<BasicData> bd = Cleaners.ConvertHistoryToBasic(history, "C");

            // validate parameters
            ValidateTema(bd, lookbackPeriod);

            // initialize
            List<EmaResult> results = new List<EmaResult>();
            List<EmaResult> emaN1 = CalcEma(bd, lookbackPeriod).ToList();

            List<BasicData> bd2 = emaN1
                .Where(x => x.Ema != null)
                .Select(x => new BasicData { Index = null, Date = x.Date, Value = (decimal)x.Ema })
                .ToList();

            List<EmaResult> emaN2 = CalcEma(bd2, lookbackPeriod).ToList();

            List<BasicData> bd3 = emaN2
                .Where(x => x.Ema != null)
                .Select(x => new BasicData { Index = null, Date = x.Date, Value = (decimal)x.Ema })
                .ToList();

            List<EmaResult> emaN3 = CalcEma(bd3, lookbackPeriod).ToList();

            // compose final results
            for (int i = 0; i < emaN1.Count; i++)
            {
                EmaResult e1 = emaN1[i];

                EmaResult result = new EmaResult
                {
                    Index = e1.Index,
                    Date = e1.Date
                };

                if (e1.Index >= 3 * lookbackPeriod - 2)
                {
                    EmaResult e2 = emaN2[e1.Index - lookbackPeriod];
                    EmaResult e3 = emaN3[e2.Index - lookbackPeriod];

                    result.Ema = 3 * e1.Ema - 3 * e2.Ema + e3.Ema;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateTema(IEnumerable<BasicData> basicData, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for DEMA.");
            }

            // check history
            int qtyHistory = basicData.Count();
            int minHistory = Math.Max(4 * lookbackPeriod, 3 * lookbackPeriod + 100);
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for DEMA.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.  "
                          + "Since this uses a smoothing technique, for a lookback period of {2}, "
                          + "we recommend you use at least {3} data points prior to the intended "
                          + "usage date for maximum precision.",
                          qtyHistory, minHistory, lookbackPeriod, 3 * lookbackPeriod + 250));
            }

        }

    }
}
