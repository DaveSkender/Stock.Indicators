using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // TRIPLE EXPONENTIAL MOVING AVERAGE
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<TemaResult> GetTripleEma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // convert quotes to basic format
            List<BasicData> bdList = quotes.ConvertToBasic(CandlePart.Close);

            // check parameter arguments
            ValidateTema(bdList, lookbackPeriods);

            // initialize
            List<TemaResult> results = new(bdList.Count);
            List<EmaResult> emaN1 = CalcEma(bdList, lookbackPeriods);

            List<BasicData> bd2 = emaN1
                .Where(x => x.Ema != null)
                .Select(x => new BasicData { Date = x.Date, Value = (decimal)x.Ema })
                .ToList();

            List<EmaResult> emaN2 = CalcEma(bd2, lookbackPeriods);

            List<BasicData> bd3 = emaN2
                .Where(x => x.Ema != null)
                .Select(x => new BasicData { Date = x.Date, Value = (decimal)x.Ema })
                .ToList();

            List<EmaResult> emaN3 = CalcEma(bd3, lookbackPeriods);

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


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<TemaResult> RemoveWarmupPeriods(
            this IEnumerable<TemaResult> results)
        {
            int n3 = results
              .ToList()
              .FindIndex(x => x.Tema != null) + 3;

            return results.Remove(n3 + 100);
        }


        // parameter validation
        private static void ValidateTema(
            IEnumerable<BasicData> quotes,
            int lookbackPeriods)
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for TEMA.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = Math.Max(4 * lookbackPeriods, 3 * lookbackPeriods + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for TEMA.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                    + "Since this uses a smoothing technique, for {2} lookback periods "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods, 3 * lookbackPeriods + 250);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
