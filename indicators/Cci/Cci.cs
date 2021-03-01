using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // COMMODITY CHANNEL INDEX
        /// <include file='./info.xml' path='indicator/*' />
        ///

        public static IEnumerable<CciResult> GetCci<TQuote>(
    IEnumerable<TQuote> history,
    int lookbackPeriod = 20)
    where TQuote : IQuote
        {
            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateCci(history, lookbackPeriod);

            // initialize
            List<CciResult> results = new List<CciResult>(historyList.Count);

            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];

                CciResult result = new CciResult
                {
                    Date = h.Date,
                    Tp = (h.High + h.Low + h.Close) / 3
                };
                results.Add(result);
            }

            List<SmaResult> smaResults = GetSma(historyList, lookbackPeriod).ToList();

            List<decimal?> meanDeviationList = new List<decimal?>();
            for (int i = 0; i < historyList.Count; i++)
            {
                if (i >= lookbackPeriod - 1)
                {
                    decimal total = 0.0m;
                    SmaResult smaResult = smaResults[i];
                    for (int p = i; p >= i - (lookbackPeriod - 1); p--)
                    {
                        total += Math.Abs(smaResult.Sma - historyList[p].Close);
                    }
                    meanDeviationList.Add(total / lookbackPeriod);

                    decimal cci = (historyList[i].Close - smaResult.Sma) / (0.015m * meanDeviationList[i].Value);
                    results[i].Cci = cci;
                }
                else
                {
                    meanDeviationList.Add(null);
                    results[i].Cci = null;
                }
            }

            return results;
        }

        private static void ValidateCci<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Commodity Channel Index.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Commodity Channel Index.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
