using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // STOCHASTIC OSCILLATOR
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<StochResult> GetStoch<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod = 14,
            int signalPeriod = 3,
            int smoothPeriod = 3)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateStoch(history, lookbackPeriod, signalPeriod, smoothPeriod);

            // initialize
            int size = historyList.Count;
            List<StochResult> results = new(size);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                StochResult result = new()
                {
                    Date = h.Date
                };

                if (index >= lookbackPeriod)
                {
                    decimal highHigh = 0;
                    decimal lowLow = decimal.MaxValue;

                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        TQuote d = historyList[p];

                        if (d.High > highHigh)
                        {
                            highHigh = d.High;
                        }

                        if (d.Low < lowLow)
                        {
                            lowLow = d.Low;
                        }
                    }

                    result.Oscillator = lowLow != highHigh
                        ? 100 * ((h.Close - lowLow) / (highHigh - lowLow))
                        : 0;
                }
                results.Add(result);
            }


            // smooth the oscillator
            if (smoothPeriod > 1)
            {
                results = SmoothOscillator(results, size, lookbackPeriod, smoothPeriod);
            }


            // signal (%D) and %J
            int stochIndex = lookbackPeriod + smoothPeriod - 2;

            for (int i = stochIndex; i < size; i++)
            {
                StochResult r = results[i];
                int index = i + 1;

                // add signal
                int signalIndex = lookbackPeriod + smoothPeriod + signalPeriod - 2;

                if (signalPeriod <= 1)
                {
                    r.Signal = r.Oscillator;
                }
                else if (index >= signalIndex)
                {
                    decimal sumOsc = 0m;
                    for (int p = index - signalPeriod; p < index; p++)
                    {
                        StochResult d = results[p];
                        sumOsc += (decimal)d.Oscillator;
                    }

                    r.Signal = sumOsc / signalPeriod;
                    r.PercentJ = (3 * r.Oscillator) - (2 * r.Signal);
                }
            }

            return results;
        }


        private static List<StochResult> SmoothOscillator(
            List<StochResult> results, int size, int lookbackPeriod, int smoothPeriod)
        {

            // temporarily store interim smoothed oscillator
            int smoothIndex = lookbackPeriod + smoothPeriod - 2;
            decimal?[] smooth = new decimal?[size]; // smoothed value

            for (int i = smoothIndex; i < size; i++)
            {
                int index = i + 1;

                decimal sumOsc = 0m;
                for (int p = index - smoothPeriod; p < index; p++)
                {
                    sumOsc += (decimal)results[p].Oscillator;
                }

                smooth[i] = sumOsc / smoothPeriod;
            }

            // replace oscillator
            for (int i = 0; i < size; i++)
            {
                results[i].Oscillator = (smooth[i] != null) ? smooth[i] : null;
            }

            return results;
        }


        private static void ValidateStoch<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod,
            int signalPeriod,
            int smoothPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Stochastic.");
            }

            if (signalPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriod), signalPeriod,
                    "Signal period must be greater than 0 for Stochastic.");
            }

            if (smoothPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothPeriod), smoothPeriod,
                    "Smooth period must be greater than 0 for Stochastic.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + smoothPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Stochastic.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
