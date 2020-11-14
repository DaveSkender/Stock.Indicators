﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // CHAIKIN MONEY FLOW
        public static IEnumerable<CmfResult> GetCmf(IEnumerable<Quote> history, int lookbackPeriod = 20)
        {

            // clean quotes
            List<Quote> historyList = Cleaners.PrepareHistory(history).ToList();

            // check parameters
            ValidateCmf(history, lookbackPeriod);

            // initialize
            List<CmfResult> results = new List<CmfResult>();
            List<AdlResult> adlResults = GetAdl(history).ToList();

            // roll through history
            for (int i = 0; i < adlResults.Count; i++)
            {
                AdlResult r = adlResults[i];

                CmfResult result = new CmfResult
                {
                    Index = r.Index,
                    Date = r.Date,
                    MoneyFlowMultiplier = r.MoneyFlowMultiplier,
                    MoneyFlowVolume = r.MoneyFlowVolume
                };

                if (r.Index >= lookbackPeriod)
                {
                    decimal sumMfv = 0;
                    decimal sumVol = 0;

                    for (int p = r.Index - lookbackPeriod; p < r.Index; p++)
                    {
                        Quote h = historyList[p];
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


        private static void ValidateCmf(IEnumerable<Quote> history, int lookbackPeriod)
        {

            // check parameters
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
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }

        }
    }

}
