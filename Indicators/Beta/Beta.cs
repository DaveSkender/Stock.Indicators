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
            historyEval = Cleaners.PrepareHistory(historyEval);

            // validate parameters
            ValidateBeta(historyMarket, historyEval, lookbackPeriod);

            // initialize results
            List<BetaResult> results = new List<BetaResult>();

            // get prerequisite data
            IEnumerable<CorrResult> correlation = GetCorrelation(historyMarket, historyEval, lookbackPeriod);

            // roll through history for interim data
            foreach (Quote e in historyEval)
            {

                BetaResult result = new BetaResult
                {
                    Index = (int)e.Index,
                    Date = e.Date
                };

                // calculate beta, if available
                CorrResult c = correlation.Where(x => x.Date == e.Date).FirstOrDefault();

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
                throw new BadParameterException("Lookback period must be greater than 0 for Beta.");
            }

            // check history
            int qtyHistory = historyEval.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Beta.  " +
                        string.Format("You provided {0} periods of history when {1} is required."
                          , qtyHistory, minHistory));
            }

            int qtyMarket = historyMarket.Count();
            if (qtyMarket < qtyHistory)
            {
                throw new BadHistoryException(
                    "Evaluation history should have at least as many records as Market history for Beta.");
            }

        }
    }

}
