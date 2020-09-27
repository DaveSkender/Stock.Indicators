using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // PRICE RELATIVE STRENGTH
        public static IEnumerable<PrsResult> GetPrs(
            IEnumerable<Quote> historyBase, IEnumerable<Quote> historyEval, int? lookbackPeriod = null, int? smaPeriod = null)
        {
            // clean quotes
            List<Quote> historyBaseList = Cleaners.PrepareHistory(historyBase).ToList();
            List<Quote> historyEvalList = Cleaners.PrepareHistory(historyEval).ToList();

            // validate parameters
            ValidatePriceRelative(historyBase, historyEval, lookbackPeriod, smaPeriod);

            // initialize
            List<PrsResult> results = new List<PrsResult>();


            // roll through history for interim data
            for (int i = 0; i < historyEvalList.Count; i++)
            {
                Quote bi = historyBaseList[i];
                Quote ei = historyEvalList[i];

                if (ei.Date != bi.Date)
                {
                    throw new BadHistoryException(
                        "Date sequence does not match.  Price Relative requires matching dates in provided histories.");
                }

                PrsResult r = new PrsResult
                {
                    Index = (int)ei.Index,
                    Date = ei.Date,
                    Prs = ei.Close / bi.Close  // relative strength ratio

                };
                results.Add(r);

                if (lookbackPeriod != null && r.Index > lookbackPeriod)
                {
                    Quote bo = historyBaseList[i - (int)lookbackPeriod];
                    Quote eo = historyEvalList[i - (int)lookbackPeriod];

                    if (bo.Close != 0 && eo.Close != 0)
                    {
                        decimal pctB = (bi.Close - bo.Close) / bo.Close;
                        decimal pctE = (ei.Close - eo.Close) / eo.Close;

                        r.PrsPercent = pctE - pctB;
                    }
                }

                // optional moving average of PRS
                if (smaPeriod != null && r.Index >= smaPeriod)
                {
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


        private static void ValidatePriceRelative(
            IEnumerable<Quote> historyBase, IEnumerable<Quote> historyEval, int? lookbackPeriod, int? smaPeriod)
        {

            // check parameters
            if (lookbackPeriod != null && lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for Price Relative Strength.");
            }

            if (smaPeriod != null && smaPeriod <= 0)
            {
                throw new BadParameterException("SMA period must be greater than 0 for Price Relative Strength.");
            }

            // check history

            int qtyHistoryEval = historyEval.Count();
            int qtyHistoryBase = historyBase.Count();

            int? minHistory = lookbackPeriod;
            if (minHistory != null && qtyHistoryEval < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Price Relative Strength.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistoryEval, minHistory));
            }

            if (qtyHistoryBase != qtyHistoryEval)
            {
                throw new BadHistoryException(
                    "Base history should have at least as many records as Eval history for Price Relative.");
            }

            // NOTE: history of less than 1 is caught in the Cleaner
        }
    }

}
