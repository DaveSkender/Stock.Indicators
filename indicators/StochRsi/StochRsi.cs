using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // STOCHASTIC RSI
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<StochRsiResult> GetStochRsi<TQuote>(
            this IEnumerable<TQuote> history,
            int rsiPeriods,
            int stochPeriods,
            int signalPeriods,
            int smoothPeriods = 1)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateStochRsi(history, rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

            // initialize
            List<RsiResult> rsiResults = GetRsi(history, rsiPeriods).ToList();
            List<StochRsiResult> results = new(rsiResults.Count);

            // convert rsi to quote format
            List<Quote> rsiQuotes = rsiResults
                .Where(x => x.Rsi != null)
                .Select(x => new Quote
                {
                    Date = x.Date,
                    High = (decimal)x.Rsi,
                    Low = (decimal)x.Rsi,
                    Close = (decimal)x.Rsi
                })
                .ToList();

            // get Stochastic of RSI
            List<StochResult> stoResults = GetStoch(rsiQuotes, stochPeriods, signalPeriods, smoothPeriods).ToList();

            // compose
            for (int i = 0; i < rsiResults.Count; i++)
            {
                RsiResult r = rsiResults[i];
                int index = i + 1;

                StochRsiResult result = new()
                {
                    Date = r.Date
                };

                if (index >= rsiPeriods + stochPeriods)
                {
                    StochResult sto = stoResults[index - rsiPeriods - 1];

                    result.StochRsi = sto.Oscillator;
                    result.Signal = sto.Signal;
                }

                results.Add(result);
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<StochRsiResult> PruneWarmupPeriods(
            this IEnumerable<StochRsiResult> results)
        {
            int n = results
                .ToList()
                .FindIndex(x => x.StochRsi != null) + 2;

            return results.Prune(n + 100);
        }


        // parameter validation
        private static void ValidateStochRsi<TQuote>(
            IEnumerable<TQuote> history,
            int rsiPeriods,
            int stochPeriods,
            int signalPeriods,
            int smoothPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (rsiPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rsiPeriods), rsiPeriods,
                    "RSI periods must be greater than 0 for Stochastic RSI.");
            }

            if (stochPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stochPeriods), stochPeriods,
                    "STOCH periods must be greater than 0 for Stochastic RSI.");
            }

            if (signalPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                    "Signal periods must be greater than 0 for Stochastic RSI.");
            }

            if (smoothPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                    "Smooth periods must be greater than 0 for Stochastic RSI.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(rsiPeriods + stochPeriods + smoothPeriods, rsiPeriods + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Stochastic RSI.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least {2} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, Math.Max(10 * rsiPeriods, minHistory));

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
