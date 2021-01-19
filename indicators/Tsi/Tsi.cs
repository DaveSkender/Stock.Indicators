using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // TRUE STRENGTH INDEX (TSI)
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<TsiResult> GetTsi<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod = 25,
            int smoothPeriod = 13,
            int signalPeriod = 7)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateTsi(history, lookbackPeriod, smoothPeriod, signalPeriod);

            // initialize
            int size = historyList.Count;
            decimal mult1 = 2m / (lookbackPeriod + 1);
            decimal mult2 = 2m / (smoothPeriod + 1);
            decimal multS = 2m / (signalPeriod + 1);
            decimal? sumS = 0m;

            List<TsiResult> results = new List<TsiResult>(size);

            decimal[] c = new decimal[size]; // price change
            decimal[] cs1 = new decimal[size]; // smooth 1
            decimal[] cs2 = new decimal[size]; // smooth 2
            decimal sumC = 0m;
            decimal sumC1 = 0m;

            decimal[] a = new decimal[size]; // abs of price change
            decimal[] as1 = new decimal[size]; // smooth 1
            decimal[] as2 = new decimal[size]; // smooth 2
            decimal sumA = 0m;
            decimal sumA1 = 0m;

            // roll through history
            for (int i = 0; i < size; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                TsiResult r = new TsiResult
                {
                    Date = h.Date
                };
                results.Add(r);

                // skip first period
                if (i == 0)
                {
                    continue;
                }

                // price change
                c[i] = h.Close - historyList[i - 1].Close;
                a[i] = Math.Abs(c[i]);

                // smoothing
                if (index > lookbackPeriod + 1)
                {
                    // first smoothing
                    cs1[i] = (c[i] - cs1[i - 1]) * mult1 + cs1[i - 1];
                    as1[i] = (a[i] - as1[i - 1]) * mult1 + as1[i - 1];

                    // second smoothing
                    if (index > lookbackPeriod + smoothPeriod)
                    {
                        cs2[i] = (cs1[i] - cs2[i - 1]) * mult2 + cs2[i - 1];
                        as2[i] = (as1[i] - as2[i - 1]) * mult2 + as2[i - 1];

                        r.Tsi = (as2[i] != 0) ? 100 * cs2[i] / as2[i] : null;

                        // signal line
                        if (signalPeriod > 0)
                        {
                            if (index >= lookbackPeriod + smoothPeriod + signalPeriod)
                            {
                                r.Signal = (r.Tsi - results[i - 1].Signal) * multS
                                         + results[i - 1].Signal;
                            }

                            // initialize signal
                            else
                            {
                                sumS += r.Tsi;

                                if (index == lookbackPeriod + smoothPeriod + signalPeriod - 1)
                                {
                                    r.Signal = sumS / signalPeriod;
                                }
                            }
                        }
                    }

                    // prepare second smoothing
                    else
                    {
                        sumC1 += cs1[i];
                        sumA1 += as1[i];

                        // inialize second smoothing
                        if (index == lookbackPeriod + smoothPeriod)
                        {
                            cs2[i] = sumC1 / smoothPeriod;
                            as2[i] = sumA1 / smoothPeriod;

                            r.Tsi = (as2[i] != 0) ? 100 * cs2[i] / as2[i] : null;
                            sumS = r.Tsi;
                        }
                    }
                }

                // prepare first smoothing
                else
                {
                    sumC += c[i];
                    sumA += a[i];

                    // initialize first smoothing
                    if (index == lookbackPeriod + 1)
                    {
                        cs1[i] = sumC / lookbackPeriod;
                        as1[i] = sumA / lookbackPeriod;

                        sumC1 = cs1[i];
                        sumA1 = as1[i];
                    }
                }
            }
            return results;
        }


        private static void ValidateTsi<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod,
            int smoothPeriod,
            int signalPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for TSI.");
            }

            if (smoothPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothPeriod), smoothPeriod,
                    "Smoothing period must be greater than 0 for TSI.");
            }

            if (signalPeriod < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriod), signalPeriod,
                    "Signal period must be greater than or equal to 0 for TSI.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + smoothPeriod + 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for TSI.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a double smoothing technique, for an N+M period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriod + smoothPeriod,
                    lookbackPeriod + smoothPeriod + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
