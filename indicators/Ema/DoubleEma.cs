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
        public static IEnumerable<EmaResult> GetDoubleEma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // convert history to basic format
            List<BasicData> bdList = history.ConvertToBasic("C");

            // check parameter arguments
            ValidateDema(bdList, lookbackPeriod);

            // initialize
            List<EmaResult> results = new(bdList.Count);
            List<EmaResult> emaN = CalcEma(bdList, lookbackPeriod).ToList();

            List<BasicData> bd2 = emaN
                .Where(x => x.Ema != null)
                .Select(x => new BasicData { Date = x.Date, Value = (decimal)x.Ema })
                .ToList();  // note: ToList seems to be required when changing data

            List<EmaResult> emaN2 = CalcEma(bd2, lookbackPeriod).ToList();

            // compose final results
            for (int i = 0; i < emaN.Count; i++)
            {
                EmaResult e1 = emaN[i];
                int index = i + 1;

                EmaResult result = new()
                {
                    Date = e1.Date
                };

                if (index >= 2 * lookbackPeriod - 1)
                {
                    EmaResult e2 = emaN2[index - lookbackPeriod];
                    result.Ema = 2 * e1.Ema - e2.Ema;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateDema(
            IEnumerable<BasicData> history,
            int lookbackPeriod)
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for DEMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(3 * lookbackPeriod, 2 * lookbackPeriod + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for DEMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for a lookback period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriod, 2 * lookbackPeriod + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
