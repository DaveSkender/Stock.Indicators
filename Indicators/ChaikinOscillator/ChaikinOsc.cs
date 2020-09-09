using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE
        public static IEnumerable<ChaikinOscResult> GetChaikinOsc(
            IEnumerable<Quote> history,
            int fastPeriod = 3,
            int slowPeriod = 10)
        {

            // clean quotes
            Cleaners.PrepareHistory(history);

            // check parameters
            ValidateChaikinOsc(history, fastPeriod, slowPeriod);

            // initialize
            List<ChaikinOscResult> results = new List<ChaikinOscResult>();
            IEnumerable<AdlResult> adlResults = GetAdl(history);


            // EMA of ADL
            IEnumerable<BasicData> adlBasicData = adlResults
                .Select(x => new BasicData { Index = x.Index, Date = x.Date, Value = x.Adl });

            IEnumerable<EmaResult> adlEmaSlow = CalcEma(adlBasicData, slowPeriod);
            IEnumerable<EmaResult> adlEmaFast = CalcEma(adlBasicData, fastPeriod);


            // roll through history
            foreach (AdlResult r in adlResults)
            {

                ChaikinOscResult result = new ChaikinOscResult
                {
                    Index = r.Index,
                    Date = r.Date,
                    MoneyFlowMultiplier = r.MoneyFlowMultiplier,
                    MoneyFlowVolume = r.MoneyFlowVolume,
                    Adl = r.Adl
                };

                // add Oscillator
                if (r.Index >= slowPeriod)
                {
                    EmaResult f = adlEmaFast.Where(x => x.Index == r.Index).FirstOrDefault();
                    EmaResult s = adlEmaSlow.Where(x => x.Index == r.Index).FirstOrDefault();

                    result.Oscillator = f.Ema - s.Ema;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateChaikinOsc(IEnumerable<Quote> history, int fastPeriod, int slowPeriod)
        {

            // check parameters
            if (fastPeriod <= 0)
            {
                throw new BadParameterException("Fast lookback period must be greater than 0 for Chaikin Oscillator.");
            }

            // check parameters
            if (slowPeriod <= fastPeriod)
            {
                throw new BadParameterException("Slow lookback period must be greater than Fast lookback period for Chaikin Oscillator.");
            }


            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(2 * slowPeriod, slowPeriod + 100);
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Chaikin Oscillator.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.  "
                          + "Since this uses a smoothing technique, for a slow period of {2}, "
                          + "we recommend you use at least {3} data points prior to the intended "
                          + "usage date for maximum precision.",
                          qtyHistory, minHistory, slowPeriod, slowPeriod + 250));
            }

        }
    }

}
