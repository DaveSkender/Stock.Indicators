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

    }

}
