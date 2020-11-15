using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // MOVING AVERAGE CONVERGENCE/DIVERGENCE (MACD) OSCILLATOR
        public static IEnumerable<MacdResult> GetMacd(IEnumerable<Quote> history, int fastPeriod = 12, int slowPeriod = 26, int signalPeriod = 9)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check parameters
            ValidateMacd(history, fastPeriod, slowPeriod, signalPeriod);

            // initialize
            List<Quote> historyList = history.ToList();
            List<EmaResult> emaFast = GetEma(history, fastPeriod).ToList();
            List<EmaResult> emaSlow = GetEma(history, slowPeriod).ToList();

            List<BasicData> emaDiff = new List<BasicData>();
            List<MacdResult> results = new List<MacdResult>();

            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];
                EmaResult df = emaFast[i];
                EmaResult ds = emaSlow[i];

                MacdResult result = new MacdResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (df?.Ema != null && ds?.Ema != null)
                {

                    decimal macd = (decimal)df.Ema - (decimal)ds.Ema;
                    result.Macd = macd;

                    // temp data for interim EMA of macd
                    BasicData diff = new BasicData
                    {
                        Index = h.Index - slowPeriod + 1,
                        Date = h.Date,
                        Value = macd
                    };

                    emaDiff.Add(diff);
                }

                results.Add(result);
            }

            // add signal and histogram to result
            List<EmaResult> emaSignal = CalcEma(emaDiff, signalPeriod).ToList();

            foreach (MacdResult r in results.Where(x => x.Index >= slowPeriod))
            {
                EmaResult ds = emaSignal[r.Index - slowPeriod];

                r.Signal = ds.Ema;
                r.Histogram = r.Macd - r.Signal;
            }

            return results;
        }


        private static void ValidateMacd(IEnumerable<Quote> history, int fastPeriod, int slowPeriod, int signalPeriod)
        {

            // check parameters
            if (fastPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fastPeriod), fastPeriod,
                    "Fast period must be greater than 0 for MACD.");
            }

            if (slowPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriod), slowPeriod,
                    "Slow period must be greater than 0 for MACD.");
            }

            if (signalPeriod < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriod), signalPeriod,
                    "Signal period must be greater than or equal to 0 for MACD.");
            }


            if (slowPeriod < fastPeriod)
            {
                throw new ArgumentOutOfRangeException(nameof(fastPeriod), fastPeriod,
                    "Fast period must be smaller than the slow period for MACD.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2 * slowPeriod + signalPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for MACD.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least 250 data points prior to the intended "
                    + "usage date for maximum precision.", qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }

        }
    }

}
