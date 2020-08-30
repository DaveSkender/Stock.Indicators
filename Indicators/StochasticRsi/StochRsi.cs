using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // STOCHASTIC RSI
        public static IEnumerable<StochRsiResult> GetStochRsi(IEnumerable<Quote> history,
            int rsiPeriod, int stochPeriod, int signalPeriod, int smoothPeriod = 1)
        {

            // clean quotes
            Cleaners.PrepareHistory(history);

            // validate parameters
            ValidateStochRsi(history, rsiPeriod, stochPeriod, signalPeriod, smoothPeriod);

            // initialize
            List<StochRsiResult> results = new List<StochRsiResult>();

            // get RSI
            IEnumerable<RsiResult> rsiResults = GetRsi(history, rsiPeriod);

            // convert rsi to quote format
            List<Quote> rsiQuotes = rsiResults
                .Where(x => x.Rsi != null)
                .Select(x => new Quote
                {
                    Index = null,
                    Date = x.Date,
                    High = (decimal)x.Rsi,
                    Low = (decimal)x.Rsi,
                    Close = (decimal)x.Rsi
                })
                .ToList();

            // get Stochastic of RSI
            IEnumerable<StochResult> stoResults = GetStoch(rsiQuotes, stochPeriod, signalPeriod, smoothPeriod);

            // compose
            foreach (RsiResult r in rsiResults)
            {

                StochRsiResult result = new StochRsiResult
                {
                    Index = r.Index,
                    Date = r.Date
                };

                if (r.Index >= rsiPeriod + stochPeriod)
                {
                    StochResult sto = stoResults
                        .Where(x => x.Index == r.Index - stochPeriod)
                        .FirstOrDefault();

                    result.StochRsi = sto.Oscillator;
                    result.Signal = sto.Signal;
                    result.IsIncreasing = sto.IsIncreasing;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateStochRsi(IEnumerable<Quote> history,
            int rsiPeriod, int stochPeriod, int signalPeriod, int smoothPeriod)
        {

            // check parameters
            if (rsiPeriod <= 0)
            {
                throw new BadParameterException("RSI period must be greater than 0 for Stochastic RSI.");
            }

            if (stochPeriod <= 0)
            {
                throw new BadParameterException("STOCH period must be greater than 0 for Stochastic RSI.");
            }

            if (signalPeriod <= 0)
            {
                throw new BadParameterException("Signal period must be greater than 0 for Stochastic RSI.");
            }

            if (smoothPeriod <= 0)
            {
                throw new BadParameterException("Smooth period must be greater than 0 for Stochastic RSI.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = rsiPeriod + stochPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Stochastic RSI.  " +
                        string.Format(cultureProvider,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }
        }
    }

}
