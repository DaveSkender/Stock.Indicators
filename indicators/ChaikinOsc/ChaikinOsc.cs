using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // CHAIKIN OSCILLATOR
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<ChaikinOscResult> GetChaikinOsc<TQuote>(
            IEnumerable<TQuote> history,
            int fastPeriod = 3,
            int slowPeriod = 10)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateChaikinOsc(history, fastPeriod, slowPeriod);

            // money flow
            List<ChaikinOscResult> results = GetAdl(history)
                .Select(r => new ChaikinOscResult
                {
                    Date = r.Date,
                    MoneyFlowMultiplier = r.MoneyFlowMultiplier,
                    MoneyFlowVolume = r.MoneyFlowVolume,
                    Adl = r.Adl
                })
                .ToList();

            // EMA of ADL
            List<BasicData> adlBasicData = results
                .Select(x => new BasicData { Date = x.Date, Value = x.Adl })
                .ToList();

            List<EmaResult> adlEmaSlow = CalcEma(adlBasicData, slowPeriod).ToList();
            List<EmaResult> adlEmaFast = CalcEma(adlBasicData, fastPeriod).ToList();

            // add Oscillator
            for (int i = slowPeriod - 1; i < results.Count; i++)
            {
                ChaikinOscResult r = results[i];

                EmaResult f = adlEmaFast[i];
                EmaResult s = adlEmaSlow[i];

                r.Oscillator = f.Ema - s.Ema;
            }

            return results;
        }


        private static void ValidateChaikinOsc<TQuote>(
            IEnumerable<TQuote> history,
            int fastPeriod,
            int slowPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (fastPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fastPeriod), fastPeriod,
                    "Fast lookback period must be greater than 0 for Chaikin Oscillator.");
            }

            if (slowPeriod <= fastPeriod)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriod), slowPeriod,
                    "Slow lookback period must be greater than Fast lookback period for Chaikin Oscillator.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(2 * slowPeriod, slowPeriod + 100);
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Chaikin Oscillator.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for a slow period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, slowPeriod, slowPeriod + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
