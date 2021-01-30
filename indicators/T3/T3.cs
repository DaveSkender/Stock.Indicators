using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // TILLSON T3 MOVING AVERAGE (T3)
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<T3Result> GetT3<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod = 5,
            double volumeFactor = 0.7)
            where TQuote : IQuote
        {

            // convert history to basic format
            List<BasicData> bdListE1 = history.ConvertToBasic("C");

            // check parameter arguments
            ValidateT3(history, lookbackPeriod, volumeFactor);

            // initialize
            int size = bdListE1.Count;
            List<T3Result> results = new List<T3Result>(size);

            decimal a = (decimal)volumeFactor;
            decimal c1 = -a * a * a;
            decimal c2 = 3 * a * a + 3 * a * a * a;
            decimal c3 = -6 * a * a - 3 * a - 3 * a * a * a;
            decimal c4 = 1 + 3 * a + a * a * a + 3 * a * a;

            List<EmaResult> e1 = CalcEma(bdListE1, lookbackPeriod).ToList();

            List<BasicData> bdListE2 = e1
                .Where(x => x.Ema is not null)
                .Select(e => new BasicData { Date = e.Date, Value = (decimal)e.Ema })
                .ToList();

            List<EmaResult> e2 = CalcEma(bdListE2, lookbackPeriod).ToList();

            List<BasicData> bdListE3 = e2
                .Where(x => x.Ema is not null)
                .Select(e => new BasicData { Date = e.Date, Value = (decimal)e.Ema })
                .ToList();

            List<EmaResult> e3 = CalcEma(bdListE3, lookbackPeriod).ToList();

            List<BasicData> bdListE4 = e3
                .Where(x => x.Ema is not null)
                .Select(e => new BasicData { Date = e.Date, Value = (decimal)e.Ema })
                .ToList();

            List<EmaResult> e4 = CalcEma(bdListE4, lookbackPeriod).ToList();

            List<BasicData> bdListE5 = e4
                .Where(x => x.Ema is not null)
                .Select(e => new BasicData { Date = e.Date, Value = (decimal)e.Ema })
                .ToList();

            List<EmaResult> e5 = CalcEma(bdListE5, lookbackPeriod).ToList();

            List<BasicData> bdListE6 = e5
                .Where(x => x.Ema is not null)
                .Select(e => new BasicData { Date = e.Date, Value = (decimal)e.Ema })
                .ToList();

            List<EmaResult> e6 = CalcEma(bdListE6, lookbackPeriod).ToList();

            // roll through history
            for (int i = 0; i < size; i++)
            {
                T3Result r = new T3Result
                {
                    Date = bdListE1[i].Date
                };

                if (i >= 6 * (lookbackPeriod - 1))
                {
                    r.T3 = c1 * e6[i - 5 * (lookbackPeriod - 1)].Ema
                         + c2 * e5[i - 4 * (lookbackPeriod - 1)].Ema
                         + c3 * e4[i - 3 * (lookbackPeriod - 1)].Ema
                         + c4 * e3[i - 2 * (lookbackPeriod - 1)].Ema;
                }

                results.Add(r);
            }

            return results;
        }


        private static void ValidateT3<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod,
            double volumeFactor)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for T3.");
            }

            if (volumeFactor <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(volumeFactor), volumeFactor,
                    "Volume Factor must be greater than 0 for T3.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = 6 * (lookbackPeriod - 1) + 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for T3.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a double smoothing technique, for a lookback period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriod,
                    6 * lookbackPeriod + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
