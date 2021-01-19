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
            IEnumerable<TQuote> history,
            int rsiPeriod,
            int stochPeriod,
            int signalPeriod,
            int smoothPeriod = 1)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateStochRsi(history, rsiPeriod, stochPeriod, signalPeriod, smoothPeriod);

            // initialize
            List<RsiResult> rsiResults = GetRsi(history, rsiPeriod).ToList();
            List<StochRsiResult> results = new List<StochRsiResult>(rsiResults.Count);

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
            List<StochResult> stoResults = GetStoch(rsiQuotes, stochPeriod, signalPeriod, smoothPeriod).ToList();

            // compose
            for (int i = 0; i < rsiResults.Count; i++)
            {
                RsiResult r = rsiResults[i];
                int index = i + 1;

                StochRsiResult result = new StochRsiResult
                {
                    Date = r.Date
                };

                if (index >= rsiPeriod + stochPeriod)
                {
                    StochResult sto = stoResults[index - rsiPeriod - 1];

                    result.StochRsi = sto.Oscillator;
                    result.Signal = sto.Signal;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateStochRsi<TQuote>(
            IEnumerable<TQuote> history,
            int rsiPeriod,
            int stochPeriod,
            int signalPeriod,
            int smoothPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (rsiPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rsiPeriod), rsiPeriod,
                    "RSI period must be greater than 0 for Stochastic RSI.");
            }

            if (stochPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stochPeriod), stochPeriod,
                    "STOCH period must be greater than 0 for Stochastic RSI.");
            }

            if (signalPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriod), signalPeriod,
                    "Signal period must be greater than 0 for Stochastic RSI.");
            }

            if (smoothPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothPeriod), smoothPeriod,
                    "Smooth period must be greater than 0 for Stochastic RSI.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(rsiPeriod + stochPeriod, rsiPeriod + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Stochastic RSI.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least {2} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, Math.Max(10 * rsiPeriod, minHistory));

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
