using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // EXPONENTIAL MOVING AVERAGE (on CLOSE price)
        /// <include file='./info.xml' path='indicators/type[@name="Main"]/*' />
        /// 
        public static IEnumerable<EmaResult> GetEma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // convert quotes to basic format
            List<BasicData> bdList = quotes.ConvertToBasic(CandlePart.Close);

            // calculate
            return bdList.CalcEma(lookbackPeriods);
        }


        // EXPONENTIAL MOVING AVERAGE (on specified OHLCV part)
        /// <include file='./info.xml' path='indicators/type[@name="Custom"]/*' />
        /// 
        public static IEnumerable<EmaResult> GetEma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            CandlePart candlePart)
            where TQuote : IQuote
        {

            // convert quotes to basic format
            List<BasicData> bdList = quotes.ConvertToBasic(candlePart);

            // calculate
            return bdList.CalcEma(lookbackPeriods);
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<EmaResult> RemoveWarmupPeriods(
            this IEnumerable<EmaResult> results)
        {
            int n = results
              .ToList()
              .FindIndex(x => x.Ema != null) + 1;

            return results.Remove(n + 100);
        }


        // standard calculation
        private static List<EmaResult> CalcEma(
            this List<BasicData> bdList, int lookbackPeriods)
        {

            // check parameter arguments
            ValidateEma(bdList, lookbackPeriods);

            // initialize
            List<EmaResult> results = new(bdList.Count);

            decimal k = 2 / (decimal)(lookbackPeriods + 1);
            decimal lastEma = 0;

            for (int i = 0; i < lookbackPeriods; i++)
            {
                lastEma += bdList[i].Value;
            }
            lastEma /= lookbackPeriods;

            // roll through quotes
            for (int i = 0; i < bdList.Count; i++)
            {
                BasicData h = bdList[i];
                int index = i + 1;

                EmaResult result = new()
                {
                    Date = h.Date
                };

                if (index > lookbackPeriods)
                {
                    result.Ema = lastEma + k * (h.Value - lastEma);
                    lastEma = (decimal)result.Ema;
                }
                else if (index == lookbackPeriods)
                {
                    result.Ema = lastEma;
                }

                results.Add(result);
            }

            return results;
        }


        // parameter validation
        private static void ValidateEma(
            List<BasicData> quotes,
            int lookbackPeriods)
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for EMA.");
            }

            // check quotes
            int qtyHistory = quotes.Count;
            int minHistory = Math.Max(2 * lookbackPeriods, lookbackPeriods + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for EMA.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                    + "Since this uses a smoothing technique, for {2} lookback periods "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods, lookbackPeriods + 250);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
