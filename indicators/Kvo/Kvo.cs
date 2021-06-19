using System;
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
            this IEnumerable<TQuote> history,
            int fastPeriod = 34,
            int slowPeriod = 55,
            int signalPeriod = 13)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateKlinger(history, fastPeriod, slowPeriod, signalPeriod);

            // initialize
            int size = historyList.Count;
            List<KvoResult> results = new(size);

            decimal[] hlc = new decimal[size];          // trend basis
            decimal[] t = new decimal[size];            // trend direction
            decimal[] dm = new decimal[size];           // daily measurement
            decimal[] cm = new decimal[size];           // cumulative measurement
            decimal?[] vf = new decimal?[size];         // volume force (VF)
            decimal?[] vfFastEma = new decimal?[size];  // EMA of VF (short-term)
            decimal?[] vfSlowEma = new decimal?[size];  // EMA of VP (long-term)

            // EMA multipliers
            decimal kFast = 2m / (fastPeriod + 1);
            decimal kSlow = 2m / (slowPeriod + 1);
            decimal kSignal = 2m / (signalPeriod + 1);

            // roll through history
            for (int i = 0; i < size; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                KvoResult r = new()
                {
                    Date = h.Date
                };
                results.Add(r);

                // trend basis comparator
                hlc[i] = h.High + h.Low + h.Close;

                // daily measurement
                dm[i] = h.High - h.Low;

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
                vf[i] = (dm[i] == cm[i] || h.Volume == 0) ? 0
                    : (dm[i] == 0) ? h.Volume * 2 * t[i] * 100m
                    : (cm[i] != 0) ? h.Volume * Math.Abs(2 * (dm[i] / cm[i] - 1)) * t[i] * 100m
                    : vf[i - 1];

                // fast-period EMA of VF
                if (index > fastPeriod + 2)
                {
                    vfFastEma[i] = vf[i] * kFast + vfFastEma[i - 1] * (1 - kFast);
                }
                else if (index == fastPeriod + 2)
                {
                    decimal? sum = 0m;
                    for (int p = 2; p <= i; p++)
                    {
                        sum += vf[p];
                    }
                    vfFastEma[i] = sum / fastPeriod;
                }

                // slow-period EMA of VF
                if (index > slowPeriod + 2)
                {
                    vfSlowEma[i] = vf[i] * kSlow + vfSlowEma[i - 1] * (1 - kSlow);
                }
                else if (index == slowPeriod + 2)
                {
                    decimal? sum = 0m;
                    for (int p = 2; p <= i; p++)
                    {
                        sum += vf[p];
                    }
                    vfSlowEma[i] = sum / slowPeriod;
                }

                // Klinger Oscillator
                if (index >= slowPeriod + 2)
                {
                    r.Oscillator = vfFastEma[i] - vfSlowEma[i];

                    // Signal
                    if (index > slowPeriod + signalPeriod + 1)
                    {
                        r.Signal = r.Oscillator * kSignal
                            + results[i - 1].Signal * (1 - kSignal);
                    }
                    else if (index == slowPeriod + signalPeriod + 1)
                    {
                        decimal? sum = 0m;
                        for (int p = slowPeriod + 1; p <= i; p++)
                        {
                            sum += results[p].Oscillator;
                        }
                        r.Signal = sum / signalPeriod;
                    }
                }
            }

            return results;
        }


        private static void ValidateKlinger<TQuote>(
            IEnumerable<TQuote> history,
            int fastPeriod,
            int slowPeriod,
            int signalPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (fastPeriod <= 2)
            {
                throw new ArgumentOutOfRangeException(nameof(fastPeriod), fastPeriod,
                    "Fast (short) period must be greater than 2 for Klinger Oscillator.");
            }

            if (slowPeriod <= fastPeriod)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriod), slowPeriod,
                    "Slow (long) period must be greater than Fast Period for Klinger Oscillator.");
            }

            if (signalPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriod), signalPeriod,
                    "Signal period must be greater than 0 for Klinger Oscillator.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = slowPeriod + 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Klinger Oscillator.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for a lookback period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, slowPeriod, slowPeriod + 150);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
