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
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 14,
            int signalPeriods = 3,
            int smoothPeriods = 3)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateStoch(quotes, lookbackPeriods, signalPeriods, smoothPeriods);

            // initialize
            int size = quotesList.Count;
            List<StochResult> results = new(size);

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                StochResult result = new()
                {
                    Date = q.Date
                };

                if (index >= lookbackPeriods)
                {
                    decimal highHigh = 0;
                    decimal lowLow = decimal.MaxValue;

                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        TQuote d = quotesList[p];

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
                        ? 100 * ((q.Close - lowLow) / (highHigh - lowLow))
                        : 0;
                }
                results.Add(result);
            }


            // smooth the oscillator
            if (smoothPeriods > 1)
            {
                results = SmoothOscillator(results, size, lookbackPeriods, smoothPeriods);
            }


            // signal (%D) and %J
            int stochIndex = lookbackPeriods + smoothPeriods - 2;

            for (int i = stochIndex; i < size; i++)
            {
                StochResult r = results[i];
                int index = i + 1;

                // add signal
                int signalIndex = lookbackPeriods + smoothPeriods + signalPeriods - 2;

                if (signalPeriods <= 1)
                {
                    r.Signal = r.Oscillator;
                }
                else if (index >= signalIndex)
                {
                    decimal sumOsc = 0m;
                    for (int p = index - signalPeriods; p < index; p++)
                    {
                        StochResult d = results[p];
                        sumOsc += (decimal)d.Oscillator;
                    }

                    r.Signal = sumOsc / signalPeriods;
                    r.PercentJ = (3 * r.Oscillator) - (2 * r.Signal);
                }
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<StochResult> RemoveWarmupPeriods(
            this IEnumerable<StochResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Oscillator != null);

            return results.Remove(removePeriods);
        }


        // internals
        private static List<StochResult> SmoothOscillator(
            List<StochResult> results, int size, int lookbackPeriods, int smoothPeriods)
        {

            // temporarily store interim smoothed oscillator
            int smoothIndex = lookbackPeriods + smoothPeriods - 2;
            decimal?[] smooth = new decimal?[size]; // smoothed value

            for (int i = smoothIndex; i < size; i++)
            {
                int index = i + 1;

                decimal sumOsc = 0m;
                for (int p = index - smoothPeriods; p < index; p++)
                {
                    sumOsc += (decimal)results[p].Oscillator;
                }

                smooth[i] = sumOsc / smoothPeriods;
            }

            // replace oscillator
            for (int i = 0; i < size; i++)
            {
                results[i].Oscillator = (smooth[i] != null) ? smooth[i] : null;
            }

            return results;
        }


        // parameter validation
        private static void ValidateStoch<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            int signalPeriods,
            int smoothPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Stochastic.");
            }

            if (signalPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                    "Signal periods must be greater than 0 for Stochastic.");
            }

            if (smoothPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                    "Smooth periods must be greater than 0 for Stochastic.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + smoothPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Stochastic.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
