using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // STOCHASTIC OSCILLATOR
        public static IEnumerable<StochResult> GetStoch(IEnumerable<Quote> history, int lookbackPeriod = 14, int signalPeriod = 3, int smoothPeriod = 3)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // validate parameters
            ValidateStoch(history, lookbackPeriod, signalPeriod, smoothPeriod);

            // initialize
            List<StochResult> results = new List<StochResult>();

            // oscillator
            foreach (Quote h in history)
            {
                StochResult result = new StochResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod)
                {

                    decimal lowLow = history.Where(x => x.Index > (h.Index - lookbackPeriod) && x.Index <= h.Index)
                                        .Select(v => v.Low)
                                        .Min();

                    decimal highHigh = history.Where(x => x.Index > (h.Index - lookbackPeriod) && x.Index <= h.Index)
                                        .Select(v => v.High)
                                        .Max();

                    if (lowLow != highHigh)
                    {
                        result.Oscillator = 100 * (float)((h.Close - lowLow) / (highHigh - lowLow));
                    }
                    else
                    {
                        result.Oscillator = 0;
                    }
                }
                results.Add(result);
            }


            // smooth the oscillator
            if (smoothPeriod > 1)
            {

                // temporarily store interim smoothed oscillator
                foreach (StochResult r in results.Where(x => x.Index >= (lookbackPeriod + smoothPeriod)))
                {
                    r.Smooth = results.Where(x => x.Index > (r.Index - smoothPeriod) && x.Index <= r.Index)
                                     .Select(v => v.Oscillator)
                                     .Average();
                }

                // replace oscillator
                foreach (StochResult r in results)
                {
                    if (r.Smooth != null)
                    {
                        r.Oscillator = (float)r.Smooth;
                    }
                    else
                    {
                        r.Oscillator = null;  // erase unsmoothed
                    }
                }
            }


            // new signal and trend info
            float lastOsc = 0;
            bool? lastIsIncreasing = null;
            foreach (StochResult r in results
                .Where(x => x.Index >= (lookbackPeriod + signalPeriod + smoothPeriod) && x.Oscillator != null))
            {
                r.Signal = results.Where(x => x.Index > (r.Index - signalPeriod) && x.Index <= r.Index)
                                 .Select(v => v.Oscillator)
                                 .Average();

                if (r.Index >= (lookbackPeriod + signalPeriod + smoothPeriod) + 1)
                {
                    if (r.Oscillator > lastOsc)
                    {
                        r.IsIncreasing = true;
                    }
                    else if (r.Oscillator < lastOsc)
                    {
                        r.IsIncreasing = false;
                    }
                    else
                    {
                        // no change, keep trend
                        r.IsIncreasing = lastIsIncreasing;
                    }
                }

                lastOsc = (float)r.Oscillator;
                lastIsIncreasing = r.IsIncreasing;
            }

            return results;
        }


        private static void ValidateStoch(IEnumerable<Quote> history, int lookbackPeriod, int signalPeriod, int smoothPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 0)
            {
                throw new BadParameterException("Lookback period must be greater than 0 for Stochastic.");
            }

            if (signalPeriod <= 0)
            {
                throw new BadParameterException("Signal period must be greater than 0 for Stochastic.");
            }

            if (smoothPeriod <= 0)
            {
                throw new BadParameterException("Smooth period must be greater than 0 for Stochastic.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Stochastic.  " +
                        string.Format("You provided {0} periods of history when at least {1} is required.", qtyHistory, minHistory));
            }
        }
    }

}
