using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // BETA COEFFICIENT
        public static IEnumerable<BetaResult> GetBeta(
            IEnumerable<Quote> historyMarket, IEnumerable<Quote> historyEval, int lookbackPeriod)
        {
            // clean quotes
            historyMarket = Cleaners.PrepareHistory(historyMarket);
            List<Quote> historyEvalList = Cleaners.PrepareHistory(historyEval).ToList();

            // validate parameters
            ValidateBeta(historyMarket, historyEval, lookbackPeriod);

            // initialize results
            List<BetaResult> results = new List<BetaResult>();

            // get prerequisite data
            List<CorrResult> correlation = GetCorrelation(historyMarket, historyEval, lookbackPeriod).ToList();

            // roll through history for interim data
            for (int i = 0; i < historyEvalList.Count; i++)
            {
                Quote e = historyEvalList[i];

                BetaResult result = new BetaResult
                {
                    Index = (int)e.Index,
                    Date = e.Date
                };

                // calculate beta, if available
                CorrResult c = correlation[i];

                if (c.Covariance != null && c.VarianceA != null && c.VarianceA != 0)
                {
                    result.Beta = c.Covariance / c.VarianceA;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateBeta(IEnumerable<Quote> historyMarket, IEnumerable<Quote> historyEval, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Beta.");
            }

            // check history
            int qtyHistoryMarket = historyMarket.Count();
            int minHistoryMarket = lookbackPeriod;
            if (qtyHistoryMarket < minHistoryMarket)
            {
                string message = "Insufficient history provided for Beta.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistoryMarket, minHistoryMarket);

                throw new BadHistoryException(nameof(historyMarket), message);
            }

            int qtyHistoryEval = historyEval.Count();
            if (qtyHistoryEval < qtyHistoryMarket)
            {
                throw new BadHistoryException(nameof(historyEval),
                    "Eval history should have at least as many records as Market history for Beta.");
            }

        }
    }

}
