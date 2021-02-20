using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // HILBERT TRANSFORM - INSTANTANEOUS TRENDLINE (HTL)
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<HtlResult> GetHtTrendline<TQuote>(
            IEnumerable<TQuote> history)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateHtTrendline(history);

            // initialize
            int size = historyList.Count;
            List<HtlResult> results = new List<HtlResult>(size);

            double[] pr = new double[size]; // price
            double[] sp = new double[size]; // smooth price
            double[] dt = new double[size]; // detrender
            double[] pd = new double[size]; // period

            double[] q1 = new double[size]; // quadrature
            double[] i1 = new double[size]; // in-phase

            double jI;
            double jQ;

            double[] q2 = new double[size]; // adj. quadrature
            double[] i2 = new double[size]; // adj. in-phase

            double[] re = new double[size];
            double[] im = new double[size];

            double[] sd = new double[size]; // smooth period
            double[] it = new double[size]; // instantaneous trend (raw)

            // roll through history
            for (int i = 0; i < size; i++)
            {
                TQuote h = historyList[i];
                pr[i] = (double)(h.High + h.Low) / 2;

                HtlResult r = new HtlResult
                {
                    Date = h.Date,
                };

                if (i > 5)
                {
                    double adj = (0.075 * pd[i - 1] + 0.54);

                    // smooth and detrender
                    sp[i] = (4 * pr[i] + 3 * pr[i - 1] + 2 * pr[i - 2] + pr[i - 3]) / 10;
                    dt[i] = (0.0962 * sp[i] + 0.5769 * sp[i - 2] - 0.5769 * sp[i - 4] - 0.0962 * sp[i - 6]) * adj;

                    // in-phase and quadrature
                    q1[i] = (0.0962 * dt[i] + 0.5769 * dt[i - 2] - 0.5769 * dt[i - 4] - 0.0962 * dt[i - 6]) * adj;
                    i1[i] = dt[i - 3];

                    // advance the phases by 90 degrees
                    jI = (0.0962 * i1[i] + 0.5769 * i1[i - 2] - 0.5769 * i1[i - 4] - 0.0962 * i1[i - 6]) * adj;
                    jQ = (0.0962 * q1[i] + 0.5769 * q1[i - 2] - 0.5769 * q1[i - 4] - 0.0962 * q1[i - 6]) * adj;

                    // phasor addition for 3-bar averaging
                    i2[i] = i1[i] - jQ;
                    q2[i] = q1[i] + jI;

                    i2[i] = 0.2 * i2[i] + 0.8 * i2[i - 1];  // smoothing it
                    q2[i] = 0.2 * q2[i] + 0.8 * q2[i - 1];

                    // homodyne discriminator
                    re[i] = i2[i] * i2[i - 1] + q2[i] * q2[i - 1];
                    im[i] = i2[i] * q2[i - 1] - q2[i] * i2[i - 1];

                    re[i] = 0.2 * re[i] + 0.8 * re[i - 1];  // smoothing it
                    im[i] = 0.2 * im[i] + 0.8 * im[i - 1];

                    // calculate period
                    if (im[i] != 0 && re[i] != 0)
                    {
                        pd[i] = 2 * Math.PI / Math.Atan(im[i] / re[i]);
                    }

                    // adjust period to thresholds
                    pd[i] = (pd[i] > 1.5 * pd[i - 1]) ? 1.5 * pd[i - 1] : pd[i];
                    pd[i] = (pd[i] < 0.67 * pd[i - 1]) ? 0.67 * pd[i - 1] : pd[i];
                    pd[i] = (pd[i] < 6d) ? 6d : pd[i];
                    pd[i] = (pd[i] > 50d) ? 50d : pd[i];

                    // smooth the period
                    pd[i] = 0.2 * pd[i] + 0.8 * pd[i - 1];
                    sd[i] = 0.33 * pd[i] + 0.67 * sd[i - 1];

                    // smooth dominant cycle period
                    int dcPeriods = (int)(Math.Truncate(sd[i] + 0.5));
                    double sumPr = 0;
                    for (int d = i - dcPeriods + 1; d <= i; d++)
                    {
                        if (d >= 0)
                        {
                            sumPr += pr[d];
                        }

                        // handle insufficient lookback history (trim scope)
                        else
                        {
                            dcPeriods--;
                        }
                    }

                    it[i] = dcPeriods > 0 ? sumPr / dcPeriods : pr[i];

                    // final indicators
                    r.Trendline = i >= 11 // 12th bar
                        ? (decimal)(4 * it[i] + 3 * it[i - 1] + 2 * it[i - 2] + it[i - 3]) / 10m
                        : (decimal)pr[i];

                    r.SmoothPrice = (decimal)(4 * pr[i] + 3 * pr[i - 1] + 2 * pr[i - 2] + pr[i - 3]) / 10m;
                }

                // initialization period
                else
                {
                    r.Trendline = (decimal)pr[i];
                    r.SmoothPrice = null;

                    pd[i] = 0;
                    sp[i] = 0;
                    dt[i] = 0;

                    i1[i] = 0;
                    q1[i] = 0;
                    i2[i] = 0;
                    q2[i] = 0;

                    re[i] = 0;
                    im[i] = 0;

                    sd[i] = 0;
                }

                results.Add(r);
            }

            return results;
        }


        private static void ValidateHtTrendline<TQuote>(
            IEnumerable<TQuote> history)
            where TQuote : IQuote
        {

            // check history
            int qtyHistory = history.Count();
            int minHistory = 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for HTL.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
