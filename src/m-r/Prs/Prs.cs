using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // PRICE RELATIVE STRENGTH
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<PrsResult> GetPrs<TQuote>(
            this IEnumerable<TQuote> historyBase,
            IEnumerable<TQuote> historyEval,
            int? lookbackPeriods = null,
            int? smaPeriods = null)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> historyBaseList = historyBase.Sort();
            List<TQuote> historyEvalList = historyEval.Sort();

            // check parameter arguments
            ValidatePriceRelative(historyBase, historyEval, lookbackPeriods, smaPeriods);

            // initialize
            List<PrsResult> results = new(historyEvalList.Count);

            // roll through quotes
            for (int i = 0; i < historyEvalList.Count; i++)
            {
                TQuote bi = historyBaseList[i];
                TQuote ei = historyEvalList[i];
                int index = i + 1;

                if (ei.Date != bi.Date)
                {
                    throw new BadQuotesException(nameof(historyEval), ei.Date,
                        "Date sequence does not match.  Price Relative requires matching dates in provided histories.");
                }

                PrsResult r = new()
                {
                    Date = ei.Date,
                    Prs = (bi.Close == 0) ? null : (double)(ei.Close / bi.Close)  // relative strength ratio
                };
                results.Add(r);

                if (lookbackPeriods != null && index > lookbackPeriods)
                {
                    TQuote bo = historyBaseList[i - (int)lookbackPeriods];
                    TQuote eo = historyEvalList[i - (int)lookbackPeriods];

                    if (bo.Close != 0 && eo.Close != 0)
                    {
                        double pctB = (double)((bi.Close - bo.Close) / bo.Close);
                        double pctE = (double)((ei.Close - eo.Close) / eo.Close);

                        r.PrsPercent = pctE - pctB;
                    }
                }

                // optional moving average of PRS
                if (smaPeriods != null && index >= smaPeriods)
                {
                    double? sumRs = 0;
                    for (int p = index - (int)smaPeriods; p < index; p++)
                    {
                        PrsResult d = results[p];
                        sumRs += d.Prs;
                    }
                    r.PrsSma = sumRs / smaPeriods;
                }
            }

            return results;
        }


        // parameter validation
        private static void ValidatePriceRelative<TQuote>(
            IEnumerable<TQuote> historyBase,
            IEnumerable<TQuote> historyEval,
            int? lookbackPeriods,
            int? smaPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods is not null and <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Price Relative Strength.");
            }

            if (smaPeriods is not null and <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                    "SMA periods must be greater than 0 for Price Relative Strength.");
            }

            // check quotes
            int qtyHistoryEval = historyEval.Count();
            int qtyHistoryBase = historyBase.Count();

            int? minHistory = lookbackPeriods;
            if (minHistory != null && qtyHistoryEval < minHistory)
            {
                string message = "Insufficient quotes provided for Price Relative Strength.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistoryEval, minHistory);

                throw new BadQuotesException(nameof(historyEval), message);
            }

            if (qtyHistoryBase != qtyHistoryEval)
            {
                throw new BadQuotesException(
                    nameof(historyBase),
                    "Base quotes should have at least as many records as Eval quotes for PRS.");
            }
        }
    }
}
