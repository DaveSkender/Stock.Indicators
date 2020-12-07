using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // AVERAGE TRUE RANGE
        public static IEnumerable<AtrResult> GetAtr<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod = 14)
            where TQuote : IQuote
        {

            // clean quotes
            List<TQuote> historyList = history.Sort();

            // validate parameters
            ValidateAtr(history, lookbackPeriod);

            // initialize results
            List<AtrResult> results = new List<AtrResult>(historyList.Count);
            decimal prevAtr = 0;
            decimal prevClose = 0;
            decimal highMinusPrevClose = 0;
            decimal lowMinusPrevClose = 0;
            decimal sumTr = 0;

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                AtrResult result = new AtrResult
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

                if (index > lookbackPeriod)
                {
                    // calculate ATR
                    result.Atr = (prevAtr * (lookbackPeriod - 1) + tr) / lookbackPeriod;
                    result.Atrp = (h.Close == 0) ? null : (result.Atr / h.Close) * 100;
                    prevAtr = (decimal)result.Atr;
                }
                else if (index == lookbackPeriod)
                {
                    // initialize ATR
                    sumTr += tr;
                    result.Atr = sumTr / lookbackPeriod;
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


        private static void ValidateAtr<TQuote>(IEnumerable<TQuote> history, int lookbackPeriod) where TQuote : IQuote
        {
            // check parameters
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for Average True Range.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for ATR.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
