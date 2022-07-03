namespace Skender.Stock.Indicators;

// MOTHER of ADAPTIVE MOVING AVERAGES - MAMA (SERIES)
public static partial class Indicator
{
    internal static List<MamaResult> CalcMama(
        this List<(DateTime, double)> tpList,
        double fastLimit,
        double slowLimit)
    {
        // check parameter arguments
        ValidateMama(fastLimit, slowLimit);

        // initialize
        int length = tpList.Count;
        List<MamaResult> results = new(length);

        double sumPr = 0d;

        double[] pr = new double[length]; // price
        double[] sm = new double[length]; // smooth
        double[] dt = new double[length]; // detrender
        double[] pd = new double[length]; // period

        double[] q1 = new double[length]; // quadrature
        double[] i1 = new double[length]; // in-phase

        double jI;
        double jQ;

        double[] q2 = new double[length]; // adj. quadrature
        double[] i2 = new double[length]; // adj. in-phase

        double[] re = new double[length];
        double[] im = new double[length];

        double[] ph = new double[length]; // phase

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];
            pr[i] = value;

            MamaResult r = new(date);
            results.Add(r);

            if (i > 5)
            {
                double adj = (0.075 * pd[i - 1]) + 0.54;

                // smooth and detrender
                sm[i] = ((4 * pr[i]) + (3 * pr[i - 1]) + (2 * pr[i - 2]) + pr[i - 3]) / 10;
                dt[i] = ((0.0962 * sm[i]) + (0.5769 * sm[i - 2]) - (0.5769 * sm[i - 4]) - (0.0962 * sm[i - 6])) * adj;

                // in-phase and quadrature
                q1[i] = ((0.0962 * dt[i]) + (0.5769 * dt[i - 2]) - (0.5769 * dt[i - 4]) - (0.0962 * dt[i - 6])) * adj;
                i1[i] = dt[i - 3];

                // advance the phases by 90 degrees
                jI = ((0.0962 * i1[i]) + (0.5769 * i1[i - 2]) - (0.5769 * i1[i - 4]) - (0.0962 * i1[i - 6])) * adj;
                jQ = ((0.0962 * q1[i]) + (0.5769 * q1[i - 2]) - (0.5769 * q1[i - 4]) - (0.0962 * q1[i - 6])) * adj;

                // phasor addition for 3-bar averaging
                i2[i] = i1[i] - jQ;
                q2[i] = q1[i] + jI;

                i2[i] = (0.2 * i2[i]) + (0.8 * i2[i - 1]);  // smoothing it
                q2[i] = (0.2 * q2[i]) + (0.8 * q2[i - 1]);

                // homodyne discriminator
                re[i] = (i2[i] * i2[i - 1]) + (q2[i] * q2[i - 1]);
                im[i] = (i2[i] * q2[i - 1]) - (q2[i] * i2[i - 1]);

                re[i] = (0.2 * re[i]) + (0.8 * re[i - 1]);  // smoothing it
                im[i] = (0.2 * im[i]) + (0.8 * im[i - 1]);

                // calculate period
                pd[i] = im[i] != 0 && re[i] != 0
                    ? 2 * Math.PI / Math.Atan(im[i] / re[i])
                    : 0d;

                // adjust period to thresholds
                pd[i] = (pd[i] > 1.5 * pd[i - 1]) ? 1.5 * pd[i - 1] : pd[i];
                pd[i] = (pd[i] < 0.67 * pd[i - 1]) ? 0.67 * pd[i - 1] : pd[i];
                pd[i] = (pd[i] < 6d) ? 6d : pd[i];
                pd[i] = (pd[i] > 50d) ? 50d : pd[i];

                // smooth the period
                pd[i] = (0.2 * pd[i]) + (0.8 * pd[i - 1]);

                // determine phase position
                ph[i] = (i1[i] != 0) ? Math.Atan(q1[i] / i1[i]) * 180 / Math.PI : 0;

                // change in phase
                double delta = Math.Max(ph[i - 1] - ph[i], 1d);

                // adaptive alpha value
                double alpha = Math.Max(fastLimit / delta, slowLimit);

                // final indicators
                r.Mama = ((alpha * pr[i]) + ((1d - alpha) * results[i - 1].Mama)).NaN2Null();
                r.Fama = ((0.5d * alpha * r.Mama) + ((1d - (0.5d * alpha)) * results[i - 1].Fama)).NaN2Null();
            }

            // initialization period
            else
            {
                sumPr += pr[i];

                if (i == 5)
                {
                    r.Mama = (sumPr / 6d).NaN2Null();
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
        }

        return results;
    }

    // parameter validation
    private static void ValidateMama(
        double fastLimit,
        double slowLimit)
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
    }
}
