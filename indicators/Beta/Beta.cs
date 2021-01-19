using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // BETA COEFFICIENT
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<BetaResult> GetBeta<TQuote>(
            IEnumerable<TQuote> historyMarket,
            IEnumerable<TQuote> historyEval,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyEvalList = historyEval.Sort();

            // check parameter arguments
            ValidateBeta(historyMarket, historyEval, lookbackPeriod);

            // initialize
            List<BetaResult> results = new List<BetaResult>(historyEvalList.Count);
            List<CorrResult> correlation = GetCorrelation(historyMarket, historyEval, lookbackPeriod).ToList();

            // roll through history
            for (int i = 0; i < historyEvalList.Count; i++)
            {
                TQuote e = historyEvalList[i];

                BetaResult result = new BetaResult
                {
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


        private static void ValidateBeta<TQuote>(
            IEnumerable<TQuote> historyMarket,
            IEnumerable<TQuote> historyEval,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
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
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistoryMarket, minHistoryMarket);

                throw new BadHistoryException(nameof(historyMarket), message);
            }

            int qtyHistoryEval = historyEval.Count();
            if (qtyHistoryEval < qtyHistoryMarket)
            {
                throw new BadHistoryException(
                    nameof(historyEval),
                    "Eval history should have at least as many records as Market history for Beta.");
            }
        }
    }
}
