using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE (on CLOSE price)
        /// <include file='./info.xml' path='indicators/type[@name="Main"]/*' />
        /// 
        public static IEnumerable<SmaResult> GetSma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateSma(quotes, lookbackPeriods);

            // initialize
            List<BasicData> bdList = quotes.ConvertToBasic(CandlePart.Close);

            // calculate
            return bdList.CalcSma(lookbackPeriods);
        }


        // SIMPLE MOVING AVERAGE (on specified OHLCV part)
        /// <include file='./info.xml' path='indicators/type[@name="Custom"]/*' />
        /// 
        public static IEnumerable<SmaResult> GetSma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            CandlePart candlePart = CandlePart.Close)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateSma(quotes, lookbackPeriods);

            // initialize
            List<BasicData> bdList = quotes.ConvertToBasic(candlePart);

            // calculate
            return bdList.CalcSma(lookbackPeriods);
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        /// 
        public static IEnumerable<SmaResult> RemoveWarmupPeriods(
            this IEnumerable<SmaResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Sma != null);

            return results.Remove(removePeriods);
        }


        // calculate
        private static IEnumerable<SmaResult> CalcSma(
            this List<BasicData> bdList,
            int lookbackPeriods)
        {

            // note: pre-validated
            // initialize
            List<SmaResult> results = new(bdList.Count);

            // roll through quotes
            for (int i = 0; i < bdList.Count; i++)
            {
                BasicData q = bdList[i];
                int index = i + 1;

                SmaResult result = new()
                {
                    Date = q.Date
                };

                if (index >= lookbackPeriods)
                {
                    decimal sumSma = 0m;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        BasicData d = bdList[p];
                        sumSma += d.Value;
                    }

                    result.Sma = sumSma / lookbackPeriods;
                }

                results.Add(result);
            }

            return results;
        }


        // parameter validation
        private static void ValidateSma<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for SMA.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for SMA.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
