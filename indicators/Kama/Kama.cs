using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // KAUFMAN's ADAPTIVE MOVING AVERAGE
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<KamaResult> GetKama<TQuote>(
            this IEnumerable<TQuote> quotes,
            int erPeriods = 10,
            int fastPeriods = 2,
            int slowPeriods = 30)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateKama(quotes, erPeriods, fastPeriods, slowPeriods);

            // initialize
            List<KamaResult> results = new(quotesList.Count);
            decimal scFast = 2m / (fastPeriods + 1);
            decimal scSlow = 2m / (slowPeriods + 1);

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                KamaResult r = new()
                {
                    Date = q.Date
                };

                if (index > erPeriods)
                {
                    // ER period change
                    decimal change = Math.Abs(q.Close - quotesList[i - erPeriods].Close);

                    // volatility
                    decimal sumPV = 0m;
                    for (int p = i - erPeriods + 1; p <= i; p++)
                    {
                        sumPV += Math.Abs(quotesList[p].Close - quotesList[p - 1].Close);
                    }

                    if (sumPV != 0)
                    {
                        // efficiency ratio
                        decimal er = change / sumPV;
                        r.ER = er;

                        // smoothing constant
                        decimal sc = er * (scFast - scSlow) + scSlow;  // squared later

                        // kama calculation
                        decimal? pk = results[i - 1].Kama;  // prior KAMA
                        r.Kama = pk + sc * sc * (q.Close - pk);
                    }

                    // handle flatline case
                    else
                    {
                        r.ER = 0;
                        r.Kama = q.Close;
                    }
                }

                // initial value
                else if (index == erPeriods)
                {
                    r.Kama = q.Close;
                }

                results.Add(r);
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<KamaResult> RemoveWarmupPeriods(
            this IEnumerable<KamaResult> results)
        {
            int erPeriods = results
                .ToList()
                .FindIndex(x => x.ER != null);

            return results.Remove(Math.Max(erPeriods + 100, 10 * erPeriods));
        }


        // parameter validation
        private static void ValidateKama<TQuote>(
            IEnumerable<TQuote> quotes,
            int erPeriods,
            int fastPeriods,
            int slowPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (erPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(erPeriods), erPeriods,
                    "Efficiency Ratio periods must be greater than 0 for KAMA.");
            }

            if (fastPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                    "Fast EMA periods must be greater than 0 for KAMA.");
            }

            if (slowPeriods <= fastPeriods)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                    "Slow EMA periods must be greater than Fast EMA period for KAMA.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = Math.Max(6 * erPeriods, erPeriods + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for KAMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for an ER period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, erPeriods, 10 * erPeriods);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
