﻿using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RELATIVE STRENGTH INDEX
        public static IEnumerable<RsiResult> GetRsi(IEnumerable<Quote> history, int lookbackPeriod = 14)
        {

            // convert history to basic format
            IEnumerable<BasicData> bd = Cleaners.ConvertHistoryToBasic(history, "C");

            // calculate
            return CalcRsi(bd, lookbackPeriod);
        }


        private static IEnumerable<RsiResult> CalcRsi(IEnumerable<BasicData> basicData, int lookbackPeriod = 14)
        {

            // clean data
            basicData = Cleaners.PrepareBasicData(basicData);

            // check parameters
            ValidateRsi(basicData, lookbackPeriod);

            // initialize
            decimal lastValue = basicData.First().Value;
            List<RsiResult> results = new List<RsiResult>();

            // load gain data
            foreach (BasicData h in basicData)
            {

                RsiResult result = new RsiResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                    Gain = (lastValue < h.Value) ? (float)(h.Value - lastValue) : 0,
                    Loss = (lastValue > h.Value) ? (float)(lastValue - h.Value) : 0
                };
                results.Add(result);

                lastValue = h.Value;
            }

            // initialize average gain
            float avgGain = results.Where(x => x.Index <= lookbackPeriod).Select(g => g.Gain).Average();
            float avgLoss = results.Where(x => x.Index <= lookbackPeriod).Select(g => g.Loss).Average();

            // initial RSI for trend analysis
            float lastRSI = (avgLoss > 0) ? 100 - (100 / (1 + (avgGain / avgLoss))) : 100;
            bool? lastIsIncreasing = null;

            // calculate RSI
            foreach (RsiResult r in results.Where(x => x.Index >= lookbackPeriod).OrderBy(d => d.Index))
            {
                avgGain = (avgGain * (lookbackPeriod - 1) + r.Gain) / lookbackPeriod;
                avgLoss = (avgLoss * (lookbackPeriod - 1) + r.Loss) / lookbackPeriod;

                if (avgLoss > 0)
                {
                    float rs = avgGain / avgLoss;
                    r.Rsi = 100 - (100 / (1 + rs));
                }
                else
                {
                    r.Rsi = 100;
                }

                if (r.Rsi > lastRSI)
                {
                    r.IsIncreasing = true;
                }
                else if (r.Rsi < lastRSI)
                {
                    r.IsIncreasing = false;
                }
                else
                {
                    // no change, keep trend
                    r.IsIncreasing = lastIsIncreasing;
                }

                lastRSI = (float)r.Rsi;
                lastIsIncreasing = r.IsIncreasing;
            }

            return results;
        }


        private static void ValidateRsi(IEnumerable<BasicData> basicData, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 1)
            {
                throw new BadParameterException("Lookback period must be greater than 1 for RSI.");
            }

            // check history
            int qtyHistory = basicData.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for RSI.  " +
                        string.Format("You provided {0} periods of history when at least {1} is required.  "
                          + "Since this uses a smoothing technique, "
                          + "we recommend you use at least 250 data points prior to the intended "
                          + "usage date for maximum precision.", qtyHistory, minHistory));
            }
        }
    }

}
