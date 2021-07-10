using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // DOUBLE EXPONENTIAL MOVING AVERAGE
        /// <include file='./info.xml' path='indicators/type[@name="DEMA"]/*' />
        /// 
        public static IEnumerable<DemaResult> GetDoubleEma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // convert quotes to basic format
            List<BasicData> bdList = quotes.ConvertToBasic("C");

            // check parameter arguments
            ValidateDema(bdList, lookbackPeriods);

            // initialize
            List<DemaResult> results = new(bdList.Count);
            List<EmaResult> emaN = CalcEma(bdList, lookbackPeriods).ToList();

            List<BasicData> bd2 = emaN
                .Where(x => x.Ema != null)
                .Select(x => new BasicData { Date = x.Date, Value = (decimal)x.Ema })
                .ToList();  // note: ToList seems to be required when changing data

            List<EmaResult> emaN2 = CalcEma(bd2, lookbackPeriods).ToList();

            // compose final results
            for (int i = 0; i < emaN.Count; i++)
            {
                EmaResult e1 = emaN[i];
                int index = i + 1;

                DemaResult result = new()
                {
                    Date = e1.Date
                };

                if (index >= 2 * lookbackPeriods - 1)
                {
                    EmaResult e2 = emaN2[index - lookbackPeriods];
                    result.Dema = 2 * e1.Ema - e2.Ema;
                }

                results.Add(result);
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<DemaResult> PruneWarmupPeriods(
            this IEnumerable<DemaResult> results)
        {
            int n2 = results
              .ToList()
              .FindIndex(x => x.Dema != null) + 2;

            return results.Prune(n2 + 100);
        }


        // parameter validation
        private static void ValidateDema(
            IEnumerable<BasicData> quotes,
            int lookbackPeriods)
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for DEMA.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = Math.Max(3 * lookbackPeriods, 2 * lookbackPeriods + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for DEMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for {2} lookback periods "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods, 2 * lookbackPeriods + 250);

                throw new BadHistoryException(nameof(quotes), message);
            }
        }
    }
}
