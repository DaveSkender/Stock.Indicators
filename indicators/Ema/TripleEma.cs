using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // TRIPLE EXPONENTIAL MOVING AVERAGE
        /// <include file='./info.xml' path='indicators/type[@name="TEMA"]/*' />
        /// 
        public static IEnumerable<TemaResult> GetTripleEma<TQuote>(
            this IEnumerable<TQuote> history,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // convert history to basic format
            List<BasicData> bdList = history.ConvertToBasic("C");

            // check parameter arguments
            ValidateTema(bdList, lookbackPeriods);

            // initialize
            List<TemaResult> results = new(bdList.Count);
            List<EmaResult> emaN1 = CalcEma(bdList, lookbackPeriods).ToList();

            List<BasicData> bd2 = emaN1
                .Where(x => x.Ema != null)
                .Select(x => new BasicData { Date = x.Date, Value = (decimal)x.Ema })
                .ToList();

            List<EmaResult> emaN2 = CalcEma(bd2, lookbackPeriods).ToList();

            List<BasicData> bd3 = emaN2
                .Where(x => x.Ema != null)
                .Select(x => new BasicData { Date = x.Date, Value = (decimal)x.Ema })
                .ToList();

            List<EmaResult> emaN3 = CalcEma(bd3, lookbackPeriods).ToList();

            // compose final results
            for (int i = 0; i < emaN1.Count; i++)
            {
                EmaResult e1 = emaN1[i];
                int index = i + 1;

                TemaResult result = new()
                {
                    Date = e1.Date
                };

                if (index >= 3 * lookbackPeriods - 2)
                {
                    EmaResult e2 = emaN2[index - lookbackPeriods];
                    EmaResult e3 = emaN3[index - 2 * lookbackPeriods + 1];

                    result.Tema = 3 * e1.Ema - 3 * e2.Ema + e3.Ema;
                }

                results.Add(result);
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<TemaResult> PruneWarmupPeriods(
            this IEnumerable<TemaResult> results)
        {
            int n3 = results
              .ToList()
              .FindIndex(x => x.Tema != null) + 3;

            return results.Prune(n3 + 100);
        }


        // parameter validation
        private static void ValidateTema(
            IEnumerable<BasicData> history,
            int lookbackPeriods)
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback period must be greater than 0 for TEMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(4 * lookbackPeriods, 3 * lookbackPeriods + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for TEMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for a lookback period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods, 3 * lookbackPeriods + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
