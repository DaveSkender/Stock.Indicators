using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // CHAIKIN MONEY FLOW
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<CmfResult> GetCmf<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod = 20)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateCmf(history, lookbackPeriod);

            // initialize
            List<CmfResult> results = new(historyList.Count);
            List<AdlResult> adlResults = GetAdl(history).ToList();

            // roll through history
            for (int i = 0; i < adlResults.Count; i++)
            {
                AdlResult r = adlResults[i];
                int index = i + 1;

                CmfResult result = new()
                {
                    Date = r.Date,
                    MoneyFlowMultiplier = r.MoneyFlowMultiplier,
                    MoneyFlowVolume = r.MoneyFlowVolume
                };

                if (index >= lookbackPeriod)
                {
                    decimal sumMfv = 0;
                    decimal sumVol = 0;

                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        TQuote h = historyList[p];
                        sumVol += h.Volume;

                        AdlResult d = adlResults[p];
                        sumMfv += d.MoneyFlowVolume;
                    }

                    decimal avgMfv = sumMfv / lookbackPeriod;
                    decimal avgVol = sumVol / lookbackPeriod;

                    if (avgVol != 0)
                    {
                        result.Cmf = avgMfv / avgVol;
                    }
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateCmf<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Chaikin Money Flow.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Chaikin Money Flow.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
