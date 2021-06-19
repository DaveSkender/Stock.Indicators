using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // MOTHER of ADAPTIVE MOVING AVERAGES (MAMA)
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<MamaResult> GetMama<TQuote>(
            this IEnumerable<TQuote> history,
            decimal fastLimit = 0.5m,
            decimal slowLimit = 0.05m)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateMama(history, fastLimit, slowLimit);

            // initialize
            int size = historyList.Count;
            List<MamaResult> results = new(size);

            double sumPr = 0d;

            double[] pr = new double[size]; // price
            double[] sm = new double[size]; // smooth
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

            double[] ph = new double[size]; // phase

            // roll through history
            for (int i = 0; i < size; i++)
            {
                TQuote h = historyList[i];
                pr[i] = (double)(h.High + h.Low) / 2;

                MamaResult r = new()
                {
                    Date = h.Date,
                };

                if (i > 5)
                {
                    double adj = (0.075 * pd[i - 1] + 0.54);

                    // smooth and detrender
                    sm[i] = (4 * pr[i] + 3 * pr[i - 1] + 2 * pr[i - 2] + pr[i - 3]) / 10;
                    dt[i] = (0.0962 * sm[i] + 0.5769 * sm[i - 2] - 0.5769 * sm[i - 4] - 0.0962 * sm[i - 6]) * adj;

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

                    // determine phase position
                    ph[i] = (i1[i] != 0) ? Math.Atan(q1[i] / i1[i]) * 180 / Math.PI : 0;

                    // change in phase
                    decimal delta = Math.Max((decimal)(ph[i - 1] - ph[i]), 1m);

                    // adaptive alpha value
                    decimal alpha = Math.Max(fastLimit / delta, slowLimit);

                    // final indicators
                    r.Mama = alpha * (decimal)pr[i] + (1m - alpha) * results[i - 1].Mama;
                    r.Fama = 0.5m * alpha * r.Mama + (1m - 0.5m * alpha) * results[i - 1].Fama;
                }

                // initialization period
                else
                {
                    sumPr += pr[i];

                    if (i == 5)
                    {
                        r.Mama = (decimal)sumPr / 6m;
                        r.Fama = r.Mama;
                    }

                    pd[i] = 0;
                    sm[i] = 0;
                    dt[i] = 0;

                    i1[i] = 0;
                    q1[i] = 0;
                    i2[i] = 0;
                    q2[i] = 0;

                    re[i] = 0;
                    im[i] = 0;

                    ph[i] = 0;
                }

                results.Add(r);
            }

            return results;
        }


        private static void ValidateMama<TQuote>(
            IEnumerable<TQuote> history,
            decimal fastLimit,
            decimal slowLimit)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (fastLimit <= slowLimit || fastLimit >= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(fastLimit), fastLimit,
                    "Fast Limit must be greater than Slow Limit and less than 1 for MAMA.");
            }

            if (slowLimit <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(slowLimit), slowLimit,
                    "Slow Limit must be greater than 0 for MAMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = 50;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for MAMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
