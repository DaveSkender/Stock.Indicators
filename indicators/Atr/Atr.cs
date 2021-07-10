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
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateAtr(quotes, lookbackPeriods);

            // initialize
            List<AtrResult> results = new(historyList.Count);
            decimal prevAtr = 0;
            decimal prevClose = 0;
            decimal highMinusPrevClose = 0;
            decimal lowMinusPrevClose = 0;
            decimal sumTr = 0;

            // roll through quotes
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                AtrResult result = new()
                {
                    Date = h.Date
                };

                if (index > 1)
                {
                    highMinusPrevClose = Math.Abs(h.High - prevClose);
                    lowMinusPrevClose = Math.Abs(h.Low - prevClose);
                }

                decimal tr = Math.Max((h.High - h.Low), Math.Max(highMinusPrevClose, lowMinusPrevClose));
                result.Tr = tr;

                if (index > lookbackPeriods)
                {
                    // calculate ATR
                    result.Atr = (prevAtr * (lookbackPeriods - 1) + tr) / lookbackPeriods;
                    result.Atrp = (h.Close == 0) ? null : (result.Atr / h.Close) * 100;
                    prevAtr = (decimal)result.Atr;
                }
                else if (index == lookbackPeriods)
                {
                    // initialize ATR
                    sumTr += tr;
                    result.Atr = sumTr / lookbackPeriods;
                    result.Atrp = (h.Close == 0) ? null : (result.Atr / h.Close) * 100;
                    prevAtr = (decimal)result.Atr;
                }
                else
                {
                    // only used for periods before ATR initialization
                    sumTr += tr;
                }

                results.Add(result);
                prevClose = h.Close;
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<AtrResult> PruneWarmupPeriods(
            this IEnumerable<AtrResult> results)
        {
            int prunePeriods = results
                .ToList()
                .FindIndex(x => x.Atr != null);

            return results.Prune(prunePeriods);
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
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least N+250 data points prior to the intended "
                    + "usage date for better precision.", qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
