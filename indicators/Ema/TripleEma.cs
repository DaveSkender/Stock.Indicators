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
        public static IEnumerable<EmaResult> GetTripleEma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // convert history to basic format
            List<BasicData> bdList = history.ConvertToBasic("C");

            // check parameter arguments
            ValidateTema(bdList, lookbackPeriod);

            // initialize
            List<EmaResult> results = new List<EmaResult>(bdList.Count);
            List<EmaResult> emaN1 = CalcEma(bdList, lookbackPeriod).ToList();

            List<BasicData> bd2 = emaN1
                .Where(x => x.Ema != null)
                .Select(x => new BasicData { Date = x.Date, Value = (decimal)x.Ema })
                .ToList();

            List<EmaResult> emaN2 = CalcEma(bd2, lookbackPeriod).ToList();

            List<BasicData> bd3 = emaN2
                .Where(x => x.Ema != null)
                .Select(x => new BasicData { Date = x.Date, Value = (decimal)x.Ema })
                .ToList();

            List<EmaResult> emaN3 = CalcEma(bd3, lookbackPeriod).ToList();

            // compose final results
            for (int i = 0; i < emaN1.Count; i++)
            {
                EmaResult e1 = emaN1[i];
                int index = i + 1;

                EmaResult result = new EmaResult
                {
                    Date = e1.Date
                };

                if (index >= 3 * lookbackPeriod - 2)
                {
                    EmaResult e2 = emaN2[index - lookbackPeriod];
                    EmaResult e3 = emaN3[index - 2 * lookbackPeriod + 1];

                    result.Ema = 3 * e1.Ema - 3 * e2.Ema + e3.Ema;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateTema(
            IEnumerable<BasicData> history,
            int lookbackPeriod)
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for TEMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(4 * lookbackPeriod, 3 * lookbackPeriod + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for TEMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for a lookback period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriod, 3 * lookbackPeriod + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
