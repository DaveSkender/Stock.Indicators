using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // TILLSON T3 MOVING AVERAGE (T3)
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<T3Result> GetT3<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod = 5,
            double volumeFactor = 0.7)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateT3(history, lookbackPeriod, volumeFactor);

            // initialize
            int size = historyList.Count;
            List<T3Result> results = new(size);

            decimal k = 2 / (decimal)(lookbackPeriod + 1);
            decimal a = (decimal)volumeFactor;
            decimal c1 = -a * a * a;
            decimal c2 = 3 * a * a + 3 * a * a * a;
            decimal c3 = -6 * a * a - 3 * a - 3 * a * a * a;
            decimal c4 = 1 + 3 * a + a * a * a + 3 * a * a;

            decimal e1 = 0, e2 = 0, e3 = 0, e4 = 0, e5 = 0, e6 = 0;
            decimal sum1 = 0, sum2 = 0, sum3 = 0, sum4 = 0, sum5 = 0, sum6 = 0;

            // roll through history
            for (int i = 0; i < size; i++)
            {
                TQuote h = historyList[i];
                T3Result r = new()
                {
                    Date = h.Date
                };

                // first smoothing
                if (i > lookbackPeriod - 1)
                {
                    e1 += k * (h.Close - e1);

                    // second smoothing
                    if (i > 2 * (lookbackPeriod - 1))
                    {
                        e2 += k * (e1 - e2);

                        // third smoothing
                        if (i > 3 * (lookbackPeriod - 1))
                        {
                            e3 += k * (e2 - e3);

                            // fourth smoothing
                            if (i > 4 * (lookbackPeriod - 1))
                            {
                                e4 += k * (e3 - e4);

                                // fifth smoothing
                                if (i > 5 * (lookbackPeriod - 1))
                                {
                                    e5 += k * (e4 - e5);

                                    // sixth smoothing
                                    if (i > 6 * (lookbackPeriod - 1))
                                    {
                                        e6 += k * (e5 - e6);

                                        // T3 moving average
                                        r.T3 = c1 * e6 + c2 * e5 + c3 * e4 + c4 * e3;
                                    }

                                    // sixth warmup
                                    else
                                    {
                                        sum6 += e5;

                                        if (i == 6 * (lookbackPeriod - 1))
                                        {
                                            e6 = sum6 / lookbackPeriod;

                                            // initial T3 moving average
                                            r.T3 = c1 * e6 + c2 * e5 + c3 * e4 + c4 * e3;
                                        }
                                    }
                                }

                                // fifth warmup
                                else
                                {
                                    sum5 += e4;

                                    if (i == 5 * (lookbackPeriod - 1))
                                    {
                                        sum6 = e5 = sum5 / lookbackPeriod;
                                    }
                                }
                            }

                            // fourth warmup
                            else
                            {
                                sum4 += e3;

                                if (i == 4 * (lookbackPeriod - 1))
                                {
                                    sum5 = e4 = sum4 / lookbackPeriod;
                                }
                            }
                        }

                        // third warmup
                        else
                        {
                            sum3 += e2;

                            if (i == 3 * (lookbackPeriod - 1))
                            {
                                sum4 = e3 = sum3 / lookbackPeriod;
                            }
                        }
                    }

                    // second warmup
                    else
                    {
                        sum2 += e1;

                        if (i == 2 * (lookbackPeriod - 1))
                        {
                            sum3 = e2 = sum2 / lookbackPeriod;
                        }
                    }
                }

                // first warmup
                else
                {
                    sum1 += h.Close;

                    if (i == lookbackPeriod - 1)
                    {
                        sum2 = e1 = sum1 / lookbackPeriod;
                    }
                }

                results.Add(r);
            }

            return results;
        }


        private static void ValidateT3<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod,
            double volumeFactor)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for T3.");
            }

            if (volumeFactor <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(volumeFactor), volumeFactor,
                    "Volume Factor must be greater than 0 for T3.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = 6 * (lookbackPeriod - 1) + 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for T3.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, for a lookback period of {2}, "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriod, 6 * (lookbackPeriod - 1) + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
