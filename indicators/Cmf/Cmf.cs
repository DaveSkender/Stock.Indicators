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
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 20)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateCmf(quotes, lookbackPeriods);

            // initialize
            List<CmfResult> results = new(historyList.Count);
            List<AdlResult> adlResults = GetAdl(quotes).ToList();

            // roll through quotes
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

                if (index >= lookbackPeriods)
                {
                    decimal sumMfv = 0;
                    decimal sumVol = 0;

                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        TQuote h = historyList[p];
                        sumVol += h.Volume;

                        AdlResult d = adlResults[p];
                        sumMfv += d.MoneyFlowVolume;
                    }

                    decimal avgMfv = sumMfv / lookbackPeriods;
                    decimal avgVol = sumVol / lookbackPeriods;

                    if (avgVol != 0)
                    {
                        result.Cmf = avgMfv / avgVol;
                    }
                }

                results.Add(result);
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<CmfResult> PruneWarmupPeriods(
            this IEnumerable<CmfResult> results)
        {
            int prunePeriods = results
              .ToList()
              .FindIndex(x => x.Cmf != null);

            return results.Prune(prunePeriods);
        }


        // parameter validation
        private static void ValidateCmf<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Chaikin Money Flow.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Chaikin Money Flow.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
