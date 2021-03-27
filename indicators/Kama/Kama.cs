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
            IEnumerable<TQuote> history,
            int erPeriod = 10,
            int fastPeriod = 2,
            int slowPeriod = 30)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateKama(history, erPeriod, fastPeriod, slowPeriod);

            // initialize
            List<KamaResult> results = new(historyList.Count);
            decimal scFast = 2m / (fastPeriod + 1);
            decimal scSlow = 2m / (slowPeriod + 1);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                KamaResult r = new()
                {
                    Date = h.Date
                };

                if (index > erPeriod)
                {
                    // ER period change
                    decimal change = Math.Abs(h.Close - historyList[i - erPeriod].Close);

                    // volatility
                    decimal sumPV = 0m;
                    for (int p = i - erPeriod + 1; p <= i; p++)
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
                else if (index == erPeriod)
                {
                    r.Kama = h.Close;
                }

                results.Add(r);
            }

            return results;
        }


        private static void ValidateKama<TQuote>(
            IEnumerable<TQuote> history,
            int erPeriod,
            int fastPeriod,
            int slowPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (erPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(erPeriod), erPeriod,
                    "Efficiency Ratio period must be greater than 0 for KAMA.");
            }

            if (fastPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fastPeriod), fastPeriod,
                    "Fast EMA period must be greater than 0 for KAMA.");
            }

            if (slowPeriod <= fastPeriod)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriod), slowPeriod,
                    "Slow EMA period must be greater than Fast EMA period for KAMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(6 * erPeriod, erPeriod + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for KAMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for an ER period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, erPeriod, 10 * erPeriod);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
