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
            Cleaners.PrepareHistory(history);

            // check parameters
            ValidateMacd(history, fastPeriod, slowPeriod, signalPeriod);

            // initialize
            IEnumerable<EmaResult> emaFast = GetEma(history, fastPeriod);
            IEnumerable<EmaResult> emaSlow = GetEma(history, slowPeriod);

            List<BasicData> emaDiff = new List<BasicData>();
            List<MacdResult> results = new List<MacdResult>();

            foreach (Quote h in history)
            {
                EmaResult df = emaFast.Where(x => x.Date == h.Date).FirstOrDefault();
                EmaResult ds = emaSlow.Where(x => x.Date == h.Date).FirstOrDefault();

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
                        Date = h.Date,
                        Index = h.Index - slowPeriod,
                        Value = macd
                    };

                    emaDiff.Add(diff);
                }

                results.Add(result);
            }

            IEnumerable<EmaResult> emaSignal = CalcEma(emaDiff, signalPeriod);

            // add signal, histogram to result
            foreach (MacdResult r in results)
            {
                EmaResult ds = emaSignal.Where(x => x.Date == r.Date).FirstOrDefault();

                if (ds?.Ema == null)
                {
                    continue;
                }

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
                throw new BadParameterException("Fast period must be greater than 0 for MACD.");
            }

            if (slowPeriod <= 0)
            {
                throw new BadParameterException("Slow period must be greater than 0 for MACD.");
            }

            if (signalPeriod < 0)
            {
                throw new BadParameterException("Signal period must be greater than or equal to 0 for MACD.");
            }


            if (slowPeriod < fastPeriod)
            {
                throw new BadParameterException("Fast period must be smaller than the slow period for MACD.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2 * slowPeriod + signalPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for MACD.  " +
                        string.Format(cultureProvider,
                        "You provided {0} periods of history when at least {1} is required.  "
                          + "Since this uses a smoothing technique, "
                          + "we recommend you use at least 250 data points prior to the intended "
                          + "usage date for maximum precision.", qtyHistory, minHistory));
            }

        }
    }

}
