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
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> historyEvalList = historyEval.Sort();

            // check parameter arguments
            ValidateBeta(historyMarket, historyEval, lookbackPeriods);

            // initialize
            List<BetaResult> results = new(historyEvalList.Count);
            List<CorrResult> correlation = GetCorrelation(historyMarket, historyEval, lookbackPeriods).ToList();

            // roll through quotes
            for (int i = 0; i < historyEvalList.Count; i++)
            {
                TQuote e = historyEvalList[i];

                BetaResult result = new()
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


        // remove recommended periods
        /// <include file='../../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<BetaResult> RemoveWarmupPeriods(
            this IEnumerable<BetaResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Beta != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateBeta<TQuote>(
            IEnumerable<TQuote> historyMarket,
            IEnumerable<TQuote> historyEval,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Beta.");
            }

            // check quotes
            int qtyHistoryMarket = historyMarket.Count();
            int minHistoryMarket = lookbackPeriods;
            if (qtyHistoryMarket < minHistoryMarket)
            {
                string message = "Insufficient quotes provided for Beta.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistoryMarket, minHistoryMarket);

                throw new BadQuotesException(nameof(historyMarket), message);
            }

            int qtyHistoryEval = historyEval.Count();
            if (qtyHistoryEval < qtyHistoryMarket)
            {
                throw new BadQuotesException(
                    nameof(historyEval),
                    "Eval quotes should have at least as many records as Market quotes for Beta.");
            }
        }
    }
}
