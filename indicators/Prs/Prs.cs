using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // PRICE RELATIVE STRENGTH
        public static IEnumerable<PrsResult> GetPrs(
            IEnumerable<Quote> historyBase, IEnumerable<Quote> historyEval, int? smaPeriod = null)
        {
            // clean quotes
            List<Quote> historyBaseList = Cleaners.PrepareHistory(historyBase).ToList();
            List<Quote> historyEvalList = Cleaners.PrepareHistory(historyEval).ToList();

            // validate parameters
            ValidatePriceRelative(historyBase, historyEval, smaPeriod);

            // initialize
            List<PrsResult> results = new List<PrsResult>();


            // roll through history for interim data
            for (int i = 0; i < historyEvalList.Count; i++)
            {
                Quote b = historyBaseList[i];
                Quote e = historyEvalList[i];

                if (e.Date != b.Date)
                {
                    throw new BadHistoryException(
                        "Date sequence does not match.  Price Relative requires matching dates in provided histories.");
                }

                PrsResult r = new PrsResult
                {
                    Index = (int)e.Index,
                    Date = e.Date,
                    Prs = e.Close / b.Close  // relative strength
                };
                results.Add(r);

                if (smaPeriod != null && r.Index >= smaPeriod)
                {
                    // get average over lookback
                    decimal sumRs = 0m;
                    for (int p = r.Index - (int)smaPeriod; p < r.Index; p++)
                    {
                        PrsResult d = results[p];
                        sumRs += (decimal)d.Prs;
                    }
                    r.Sma = sumRs / smaPeriod;
                }

            }

            return results;
        }


        private static void ValidatePriceRelative(IEnumerable<Quote> historyBase, IEnumerable<Quote> historyEval, int? smaPeriod)
        {

            // check parameters
            if (smaPeriod != null && smaPeriod <= 0)
            {
                throw new BadParameterException("Momentum lookback period must be greater than 0 for Price Relative.");
            }

            // check history
            int qtyHistoryEval = historyEval.Count();
            int minHistoryEval = 1;
            if (qtyHistoryEval < minHistoryEval)
            {
                throw new BadHistoryException("Insufficient history provided for Price Relative.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistoryEval, minHistoryEval));
            }

            int qtyHistoryBase = historyBase.Count();
            if (qtyHistoryBase != qtyHistoryEval)
            {
                throw new BadHistoryException(
                    "Base history should have at least as many records as Eval history for Price Relative.");
            }
        }
    }

}
