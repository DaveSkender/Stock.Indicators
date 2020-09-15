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
            List<Quote> historyList = Cleaners.PrepareHistory(history).ToList();

            // validate parameters
            ValidateStoch(history, lookbackPeriod, signalPeriod, smoothPeriod);

            // initialize
            List<StochResult> results = new List<StochResult>();

            // oscillator
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];

                StochResult result = new StochResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index >= lookbackPeriod)
                {
                    decimal highHigh = 0;
                    decimal lowLow = decimal.MaxValue;

                    for (int p = (int)h.Index - lookbackPeriod; p < h.Index; p++)
                    {
                        Quote d = historyList[p];

                        if (d.High > highHigh)
                        {
                            highHigh = d.High;
                        }

                        if (d.Low < lowLow)
                        {
                            lowLow = d.Low;
                        }
                    }

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
            int stochIndex = lookbackPeriod + smoothPeriod - 1;

            foreach (StochResult r in results.Where(x => x.Index >= stochIndex))
            {
                // add signal
                int signalIndex = lookbackPeriod + smoothPeriod + signalPeriod - 2;

                if (signalPeriod <= 1)
                {
                    r.Signal = r.Oscillator;
                }
                else if (r.Index >= signalIndex)
                {
                    decimal sumOsc = 0m;
                    for (int p = r.Index - signalPeriod; p < r.Index; p++)
                    {
                        StochResult d = results[p];
                        sumOsc += (decimal)d.Oscillator;
                    }

                    r.Signal = sumOsc / signalPeriod;
                }
            }

            return results;
        }


        private static List<StochResult> SmoothOscillator(List<StochResult> results, int lookbackPeriod, int smoothPeriod)
        {

            // temporarily store interim smoothed oscillator
            int smoothIndex = lookbackPeriod + smoothPeriod - 1;

            //foreach (StochResult r in results.Where(x => x.Index >= smoothIndex))
            for (int i = smoothIndex - 1; i < results.Count; i++)
            {
                StochResult r = results[i];

                decimal sumOsc = 0m;
                for (int p = r.Index - smoothPeriod; p < r.Index; p++)
                {
                    StochResult d = results[p];
                    sumOsc += (decimal)d.Oscillator;
                }

                r.Smooth = sumOsc / smoothPeriod;
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
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }
        }
    }

}
