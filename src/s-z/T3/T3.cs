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
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 5,
            double volumeFactor = 0.7)
            where TQuote : IQuote
        {

            // convert quotes
            List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.Close);

            // check parameter arguments
            ValidateT3(quotes, lookbackPeriods, volumeFactor);

            // initialize
            int size = bdList.Count;
            List<T3Result> results = new(size);

            double k = 2d / (lookbackPeriods + 1);
            double a = volumeFactor;
            double c1 = -a * a * a;
            double c2 = 3 * a * a + 3 * a * a * a;
            double c3 = -6 * a * a - 3 * a - 3 * a * a * a;
            double c4 = 1 + 3 * a + a * a * a + 3 * a * a;

            double e1 = 0, e2 = 0, e3 = 0, e4 = 0, e5 = 0, e6 = 0;
            double sum1 = 0, sum2 = 0, sum3 = 0, sum4 = 0, sum5 = 0, sum6 = 0;

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                BasicD q = bdList[i];
                T3Result r = new()
                {
                    Date = q.Date
                };

                // first smoothing
                if (i > lookbackPeriods - 1)
                {
                    e1 += k * (q.Value - e1);

                    // second smoothing
                    if (i > 2 * (lookbackPeriods - 1))
                    {
                        e2 += k * (e1 - e2);

                        // third smoothing
                        if (i > 3 * (lookbackPeriods - 1))
                        {
                            e3 += k * (e2 - e3);

                            // fourth smoothing
                            if (i > 4 * (lookbackPeriods - 1))
                            {
                                e4 += k * (e3 - e4);

                                // fifth smoothing
                                if (i > 5 * (lookbackPeriods - 1))
                                {
                                    e5 += k * (e4 - e5);

                                    // sixth smoothing
                                    if (i > 6 * (lookbackPeriods - 1))
                                    {
                                        e6 += k * (e5 - e6);

                                        // T3 moving average
                                        r.T3 = (decimal?)(c1 * e6 + c2 * e5 + c3 * e4 + c4 * e3);
                                    }

                                    // sixth warmup
                                    else
                                    {
                                        sum6 += e5;

                                        if (i == 6 * (lookbackPeriods - 1))
                                        {
                                            e6 = sum6 / lookbackPeriods;

                                            // initial T3 moving average
                                            r.T3 = (decimal?)(c1 * e6 + c2 * e5 + c3 * e4 + c4 * e3);
                                        }
                                    }
                                }

                                // fifth warmup
                                else
                                {
                                    sum5 += e4;

                                    if (i == 5 * (lookbackPeriods - 1))
                                    {
                                        sum6 = e5 = sum5 / lookbackPeriods;
                                    }
                                }
                            }

                            // fourth warmup
                            else
                            {
                                sum4 += e3;

                                if (i == 4 * (lookbackPeriods - 1))
                                {
                                    sum5 = e4 = sum4 / lookbackPeriods;
                                }
                            }
                        }

                        // third warmup
                        else
                        {
                            sum3 += e2;

                            if (i == 3 * (lookbackPeriods - 1))
                            {
                                sum4 = e3 = sum3 / lookbackPeriods;
                            }
                        }
                    }

                    // second warmup
                    else
                    {
                        sum2 += e1;

                        if (i == 2 * (lookbackPeriods - 1))
                        {
                            sum3 = e2 = sum2 / lookbackPeriods;
                        }
                    }
                }

                // first warmup
                else
                {
                    sum1 += (double)q.Value;

                    if (i == lookbackPeriods - 1)
                    {
                        sum2 = e1 = sum1 / lookbackPeriods;
                    }
                }

                results.Add(r);
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<T3Result> RemoveWarmupPeriods(
            this IEnumerable<T3Result> results)
        {
            int n6 = results
                .ToList()
                .FindIndex(x => x.T3 != null);

            return results.Remove(n6 + 250);
        }


        // parameter validation
        private static void ValidateT3<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            double volumeFactor)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for T3.");
            }

            if (volumeFactor <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(volumeFactor), volumeFactor,
                    "Volume Factor must be greater than 0 for T3.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = 6 * (lookbackPeriods - 1) + 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for T3.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                    + "Since this uses a smoothing technique, for {2} lookback periods "
                    + "we recommend you use at least {3} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods, 6 * (lookbackPeriods - 1) + 250);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
