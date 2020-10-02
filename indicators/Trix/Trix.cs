using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // TRIPLE EMA OSCILLATOR (TRIX)
        public static IEnumerable<TrixResult> GetTrix(
            IEnumerable<Quote> history, int lookbackPeriod, int? signalPeriod = null)
        {

            // convert history to basic format
            IEnumerable<BasicData> bd = Cleaners.ConvertHistoryToBasic(history, "C");

            // validate parameters
            ValidateTrix(bd, lookbackPeriod);

            // initialize
            List<TrixResult> results = new List<TrixResult>();
            decimal? lastEma = null;

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

                TrixResult result = new TrixResult
                {
                    Index = e1.Index,
                    Date = e1.Date
                };

                results.Add(result);

                if (e1.Index >= 3 * lookbackPeriod - 2)
                {
                    EmaResult e2 = emaN2[e1.Index - lookbackPeriod];
                    EmaResult e3 = emaN3[e2.Index - lookbackPeriod];

                    result.Ema3 = e3.Ema;

                    if (lastEma != null && lastEma != 0)
                    {
                        result.Trix = 100 * (e3.Ema - lastEma) / lastEma;
                    }

                    lastEma = e3.Ema;

                    // optional SMA
                    if (signalPeriod != null && e1.Index >= 3 * lookbackPeriod - 2 + signalPeriod)
                    {
                        decimal sumSma = 0m;
                        for (int p = e1.Index - (int)signalPeriod; p < e1.Index; p++)
                        {
                            sumSma += (decimal)results[p].Trix;
                        }

                        result.Signal = sumSma / signalPeriod;
                    }
                }
            }

            return results;
        }


        private static void ValidateTrix(IEnumerable<BasicData> basicData, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for TRIX.");
            }

            // check history
            int qtyHistory = basicData.Count();
            int minHistory = Math.Max(4 * lookbackPeriod, 3 * lookbackPeriod + 100);
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for TRIX.  " +
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
