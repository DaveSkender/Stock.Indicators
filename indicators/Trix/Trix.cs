using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // TRIPLE EMA OSCILLATOR (TRIX)
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<TrixResult> GetTrix<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            int? signalPeriods = null)
            where TQuote : IQuote
        {

            // convert quotes to basic format
            List<BasicData> bdList = quotes.ConvertToBasic("C");

            // check parameter arguments
            ValidateTrix(bdList, lookbackPeriods);

            // initialize
            List<TrixResult> results = new(bdList.Count);
            decimal? lastEma = null;

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

                TrixResult result = new()
                {
                    Date = e1.Date
                };

                results.Add(result);

                if (index >= 3 * lookbackPeriods - 2)
                {
                    EmaResult e2 = emaN2[index - lookbackPeriods];
                    EmaResult e3 = emaN3[index - 2 * lookbackPeriods + 1];

                    result.Ema3 = e3.Ema;

                    if (lastEma is not null and not 0)
                    {
                        result.Trix = 100 * (e3.Ema - lastEma) / lastEma;
                    }

                    lastEma = e3.Ema;

                    // optional SMA signal
                    GetTrixSignal(signalPeriods, index, lookbackPeriods, results);
                }
            }

            return results;
        }


        // remove recommended periods extensions
        public static IEnumerable<TrixResult> RemoveWarmupPeriods(
            this IEnumerable<TrixResult> results)
        {
            int n3 = results
                .ToList()
                .FindIndex(x => x.Trix != null) + 2;

            return results.Remove(n3 + 250);
        }


        // internals
        private static void GetTrixSignal(
            int? signalPeriods, int index, int lookbackPeriods, List<TrixResult> results)
        {
            if (signalPeriods != null && index >= 3 * lookbackPeriods - 2 + signalPeriods)
            {
                decimal sumSma = 0m;
                for (int p = index - (int)signalPeriods; p < index; p++)
                {
                    sumSma += (decimal)results[p].Trix;
                }

                results[index - 1].Signal = sumSma / signalPeriods;
            }
        }


        // parameter validation
        private static void ValidateTrix(
            IEnumerable<BasicData> quotes,
            int lookbackPeriods)
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for TRIX.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = Math.Max(4 * lookbackPeriods, 3 * lookbackPeriods + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for TRIX.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for {2} lookback periods "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods, 3 * lookbackPeriods + 250);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
