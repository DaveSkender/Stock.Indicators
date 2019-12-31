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

            // initialize results
            IEnumerable<EmaResult> emaFast = GetEma(history, fastPeriod);
            IEnumerable<EmaResult> emaSlow = GetEma(history, slowPeriod);

            List<Quote> emaDiff = new List<Quote>();
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
                    Quote diff = new Quote
                    {
                        Date = h.Date,
                        Index = h.Index - slowPeriod,
                        Close = macd
                    };

                    emaDiff.Add(diff);
                }

                results.Add(result);
            }

            IEnumerable<EmaResult> emaSignal = GetEma(emaDiff, signalPeriod);
            decimal? prevMacd = null;
            decimal? prevSignal = null;


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

                // trend and divergence
                if (prevMacd != null && prevSignal != null)
                {
                    r.IsBullish = (r.Macd > r.Signal) ? true : false;
                    r.IsDiverging = (Math.Abs((decimal)r.Macd - (decimal)r.Signal) > Math.Abs((decimal)prevMacd - (decimal)prevSignal)) ? true : false;
                }

                // store for next iteration
                prevMacd = r.Macd;
                prevSignal = r.Signal;
            }

            return results;
        }

    }

}
