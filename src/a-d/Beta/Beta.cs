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
            IEnumerable<TQuote> quotesMarket,
            IEnumerable<TQuote> quotesEval,
            int lookbackPeriods,
            BetaType type = BetaType.Standard)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesListEval = quotesEval.Sort();
            List<TQuote> quotesListMrkt = quotesMarket.Sort();

            // check parameter arguments
            ValidateBeta(quotesMarket, quotesEval, lookbackPeriods);

            // initialize
            List<BetaResult> results = new(quotesListEval.Count);
            bool calcSd = type is BetaType.All or BetaType.Standard;
            bool calcUp = type is BetaType.All or BetaType.Up;
            bool calcDn = type is BetaType.All or BetaType.Down;

            // roll through quotes
            for (int i = 0; i < quotesListEval.Count; i++)
            {
                TQuote e = quotesListEval[i];

                BetaResult r = new()
                {
                    Date = e.Date
                };
                results.Add(r);

                // skip warmup periods
                if (i < lookbackPeriods - 1)
                {
                    continue;
                }

                // calculate standard beta
                if (calcSd)
                {
                    r.CalcBeta(
                    i, lookbackPeriods, quotesListMrkt, quotesListEval, BetaType.Standard);
                }

                // calculate up/down betas
                if (i >= lookbackPeriods)
                {
                    if (calcDn)
                    {
                        r.CalcBeta(
                        i, lookbackPeriods, quotesListMrkt, quotesListEval, BetaType.Down);
                    }

                    if (calcUp)
                    {
                        r.CalcBeta(
                        i, lookbackPeriods, quotesListMrkt, quotesListEval, BetaType.Up);
                    }
                }

                // ratio and convexity
                if (type == BetaType.All && r.BetaUp != null && r.BetaDown != null)
                {
                    r.Ratio = (r.BetaDown != 0) ? r.BetaUp / r.BetaDown : null;
                    r.Convexity = (r.BetaUp - r.BetaDown) * (r.BetaUp - r.BetaDown);
                }
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<BetaResult> RemoveWarmupPeriods(
            this IEnumerable<BetaResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Beta != null);

            return results.Remove(removePeriods);
        }


        // calculate beta
        private static void CalcBeta<TQuote>(
            this BetaResult r,
            int index,
            int lookbackPeriods,
            List<TQuote> quotesListMrkt,
            List<TQuote> quotesListEval,
            BetaType type)
            where TQuote : IQuote
        {
            // do not supply type==BetaType.All
            if (type is BetaType.All)
            {
                return;
            }

            // initialize
            CorrResult c = new();

            List<decimal> dataA = new(lookbackPeriods);
            List<decimal> dataB = new(lookbackPeriods);

            for (int p = index - lookbackPeriods + 1; p <= index; p++)
            {
                decimal a = quotesListMrkt[p].Close;
                decimal b = quotesListEval[p].Close;

                if (type is BetaType.Standard)
                {
                    dataA.Add(a);
                    dataB.Add(b);
                }
                else if (type is BetaType.Down
                    && a < quotesListMrkt[p - 1].Close)
                {
                    dataA.Add(a);
                    dataB.Add(b);
                }
                else if (type is BetaType.Up
                    && a > quotesListMrkt[p - 1].Close)
                {
                    dataA.Add(a);
                    dataB.Add(b);
                }
            }

            if (dataA.Count > 0)
            {
                // calculate correlation, covariance, and variance 
                c.CalcCorrelation(dataA.ToArray(), dataB.ToArray());

                // calculate beta
                if (c.Covariance != null && c.VarianceA != null && c.VarianceA != 0)
                {
                    decimal? beta = c.Covariance / c.VarianceA;

                    if (type == BetaType.Standard)
                    {
                        r.Beta = beta;
                    }
                    else if (type == BetaType.Down)
                    {
                        r.BetaDown = beta;
                    }
                    else if (type == BetaType.Up)
                    {
                        r.BetaUp = beta;
                    }
                }
            }
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

