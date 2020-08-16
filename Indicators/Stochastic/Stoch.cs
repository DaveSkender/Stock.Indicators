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
                    List<Quote> period = history
                        .Where(x => x.Index > (h.Index - lookbackPeriod) && x.Index <= h.Index)
                        .ToList();

                    decimal lowLow = period.Select(v => v.Low).Min();
                    decimal highHigh = period.Select(v => v.High).Max();

                    if (lowLow != highHigh)
                    {
                        result.Oscillator = 100 * ((h.Close - lowLow) / (highHigh - lowLow));
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
                results = SmoothOscillator(results, lookbackPeriod, smoothPeriod);
            }


            // signal and period direction info
            decimal? lastOsc = null;
            bool? lastIsIncreasing = null;
            int stochIndex = lookbackPeriod + smoothPeriod - 1;

            foreach (StochResult r in results
                .Where(x => x.Index >= stochIndex))
            {
                // add signal
                int signalIndex = lookbackPeriod + smoothPeriod + signalPeriod - 2;

                if (signalPeriod <= 1)
                {
                    r.Signal = r.Oscillator;
                }
                else if (r.Index >= signalIndex)
                {
                    r.Signal = results
                        .Where(x => x.Index > (r.Index - signalPeriod) && x.Index <= r.Index)
                        .ToList()
                        .Select(v => v.Oscillator)
                        .Average();
                }

                // add direction
                if (lastOsc != null)
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

                lastOsc = (decimal)r.Oscillator;
                lastIsIncreasing = r.IsIncreasing;
            }

            return results;
        }


        private static List<StochResult> SmoothOscillator(List<StochResult> results, int lookbackPeriod, int smoothPeriod)
        {

            // temporarily store interim smoothed oscillator
            int smoothIndex = lookbackPeriod + smoothPeriod - 1;

            foreach (StochResult r in results.Where(x => x.Index >= smoothIndex))
            {
                r.Smooth = results.Where(x => x.Index > (r.Index - smoothPeriod) && x.Index <= r.Index)
                                 .ToList()
                                 .Select(v => v.Oscillator)
                                 .Average();
            }

            // replace oscillator
            foreach (StochResult r in results)
            {
                r.Oscillator = (r.Smooth != null) ? r.Smooth : null;
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
            int minHistory = lookbackPeriod + smoothPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Stochastic.  " +
                        string.Format(cultureProvider,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }
        }
    }

}
