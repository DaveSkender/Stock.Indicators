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
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateKama(quotes, erPeriods, fastPeriods, slowPeriods);

            // initialize
            List<KamaResult> results = new(historyList.Count);
            decimal scFast = 2m / (fastPeriods + 1);
            decimal scSlow = 2m / (slowPeriods + 1);

            // roll through quotes
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                KamaResult r = new()
                {
                    Date = h.Date
                };

                if (index > erPeriods)
                {
                    // ER period change
                    decimal change = Math.Abs(h.Close - historyList[i - erPeriods].Close);

                    // volatility
                    decimal sumPV = 0m;
                    for (int p = i - erPeriods + 1; p <= i; p++)
                    {
                        sumPV += Math.Abs(historyList[p].Close - historyList[p - 1].Close);
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
                        r.Kama = pk + sc * sc * (h.Close - pk);
                    }

                    // handle flatline case
                    else
                    {
                        r.ER = 0;
                        r.Kama = h.Close;
                    }
                }

                // initial value
                else if (index == erPeriods)
                {
                    r.Kama = h.Close;
                }

                results.Add(r);
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<KamaResult> PruneWarmupPeriods(
            this IEnumerable<KamaResult> results)
        {
            int erPeriods = results
                .ToList()
                .FindIndex(x => x.ER != null);

            return results.Prune(Math.Max(erPeriods + 100, 10 * erPeriods));
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
