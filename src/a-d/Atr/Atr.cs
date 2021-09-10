using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // AVERAGE TRUE RANGE
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<AtrResult> GetAtr<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 14)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateAtr(quotes, lookbackPeriods);

            // initialize
            List<AtrResult> results = new(quotesList.Count);
            decimal prevAtr = 0;
            decimal prevClose = 0;
            decimal highMinusPrevClose = 0;
            decimal lowMinusPrevClose = 0;
            decimal sumTr = 0;

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                AtrResult result = new()
                {
                    Date = q.Date
                };

                if (index > 1)
                {
                    highMinusPrevClose = Math.Abs(q.High - prevClose);
                    lowMinusPrevClose = Math.Abs(q.Low - prevClose);
                }

                decimal tr = Math.Max((q.High - q.Low), Math.Max(highMinusPrevClose, lowMinusPrevClose));
                result.Tr = tr;

                if (index > lookbackPeriods)
                {
                    // calculate ATR
                    result.Atr = (prevAtr * (lookbackPeriods - 1) + tr) / lookbackPeriods;
                    result.Atrp = (q.Close == 0) ? null : (result.Atr / q.Close) * 100;
                    prevAtr = (decimal)result.Atr;
                }
                else if (index == lookbackPeriods)
                {
                    // initialize ATR
                    sumTr += tr;
                    result.Atr = sumTr / lookbackPeriods;
                    result.Atrp = (q.Close == 0) ? null : (result.Atr / q.Close) * 100;
                    prevAtr = (decimal)result.Atr;
                }
                else
                {
                    // only used for periods before ATR initialization
                    sumTr += tr;
                }

                results.Add(result);
                prevClose = q.Close;
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<AtrResult> RemoveWarmupPeriods(
            this IEnumerable<AtrResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Atr != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateAtr<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 1 for Average True Range.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for ATR.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least N+250 data points prior to the intended "
                    + "usage date for better precision.", qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
