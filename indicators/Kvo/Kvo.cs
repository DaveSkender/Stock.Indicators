﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // KLINGER VOLUME OSCILLATOR
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<KvoResult> GetKvo<TQuote>(
            this IEnumerable<TQuote> quotes,
            int fastPeriods = 34,
            int slowPeriods = 55,
            int signalPeriods = 13)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateKlinger(quotes, fastPeriods, slowPeriods, signalPeriods);

            // initialize
            int size = quotesList.Count;
            List<KvoResult> results = new(size);

            decimal[] hlc = new decimal[size];          // trend basis
            decimal[] t = new decimal[size];            // trend direction
            decimal[] dm = new decimal[size];           // daily measurement
            decimal[] cm = new decimal[size];           // cumulative measurement
            decimal?[] vf = new decimal?[size];         // volume force (VF)
            decimal?[] vfFastEma = new decimal?[size];  // EMA of VF (short-term)
            decimal?[] vfSlowEma = new decimal?[size];  // EMA of VP (long-term)

            // EMA multipliers
            decimal kFast = 2m / (fastPeriods + 1);
            decimal kSlow = 2m / (slowPeriods + 1);
            decimal kSignal = 2m / (signalPeriods + 1);

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                KvoResult r = new()
                {
                    Date = q.Date
                };
                results.Add(r);

                // trend basis comparator
                hlc[i] = q.High + q.Low + q.Close;

                // daily measurement
                dm[i] = q.High - q.Low;

                if (i <= 0)
                {
                    continue;
                }

                // trend direction
                t[i] = (hlc[i] > hlc[i - 1]) ? 1 : -1;

                if (i <= 1)
                {
                    cm[i] = 0;
                    continue;
                }

                // cumulative measurement
                cm[i] = (t[i] == t[i - 1]) ?
                        (cm[i - 1] + dm[i]) : (dm[i - 1] + dm[i]);

                // volume force (VF)
                vf[i] = (dm[i] == cm[i] || q.Volume == 0) ? 0
                    : (dm[i] == 0) ? q.Volume * 2 * t[i] * 100m
                    : (cm[i] != 0) ? q.Volume * Math.Abs(2 * (dm[i] / cm[i] - 1)) * t[i] * 100m
                    : vf[i - 1];

                // fast-period EMA of VF
                if (index > fastPeriods + 2)
                {
                    vfFastEma[i] = vf[i] * kFast + vfFastEma[i - 1] * (1 - kFast);
                }
                else if (index == fastPeriods + 2)
                {
                    decimal? sum = 0m;
                    for (int p = 2; p <= i; p++)
                    {
                        sum += vf[p];
                    }
                    vfFastEma[i] = sum / fastPeriods;
                }

                // slow-period EMA of VF
                if (index > slowPeriods + 2)
                {
                    vfSlowEma[i] = vf[i] * kSlow + vfSlowEma[i - 1] * (1 - kSlow);
                }
                else if (index == slowPeriods + 2)
                {
                    decimal? sum = 0m;
                    for (int p = 2; p <= i; p++)
                    {
                        sum += vf[p];
                    }
                    vfSlowEma[i] = sum / slowPeriods;
                }

                // Klinger Oscillator
                if (index >= slowPeriods + 2)
                {
                    r.Oscillator = vfFastEma[i] - vfSlowEma[i];

                    // Signal
                    if (index > slowPeriods + signalPeriods + 1)
                    {
                        r.Signal = r.Oscillator * kSignal
                            + results[i - 1].Signal * (1 - kSignal);
                    }
                    else if (index == slowPeriods + signalPeriods + 1)
                    {
                        decimal? sum = 0m;
                        for (int p = slowPeriods + 1; p <= i; p++)
                        {
                            sum += results[p].Oscillator;
                        }
                        r.Signal = sum / signalPeriods;
                    }
                }
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<KvoResult> RemoveWarmupPeriods(
            this IEnumerable<KvoResult> results)
        {
            int l = results
                .ToList()
                .FindIndex(x => x.Oscillator != null) - 1;

            return results.Remove(l + 150);
        }


        // parameter validation
        private static void ValidateKlinger<TQuote>(
            IEnumerable<TQuote> quotes,
            int fastPeriods,
            int slowPeriods,
            int signalPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (fastPeriods <= 2)
            {
                throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                    "Fast (short) Periods must be greater than 2 for Klinger Oscillator.");
            }

            if (slowPeriods <= fastPeriods)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                    "Slow (long) Periods must be greater than Fast Periods for Klinger Oscillator.");
            }

            if (signalPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                    "Signal Periods must be greater than 0 for Klinger Oscillator.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = slowPeriods + 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Klinger Oscillator.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for {2} lookback periods "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, slowPeriods, slowPeriods + 150);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
