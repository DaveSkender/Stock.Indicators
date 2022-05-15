namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // TRUE STRENGTH INDEX (TSI)
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<TsiResult> GetTsi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> bdList = quotes.ToBasicClass(CandlePart.Close);

        // check parameter arguments
        ValidateTsi(lookbackPeriods, smoothPeriods, signalPeriods);

        // initialize
        int length = bdList.Count;
        double mult1 = 2d / (lookbackPeriods + 1);
        double mult2 = 2d / (smoothPeriods + 1);
        double multS = 2d / (signalPeriods + 1);
        double sumS = 0;

        List<TsiResult> results = new(length);

        double[] c = new double[length]; // price change
        double[] cs1 = new double[length]; // smooth 1
        double[] cs2 = new double[length]; // smooth 2
        double sumC = 0;
        double sumC1 = 0;

        double[] a = new double[length]; // abs of price change
        double[] as1 = new double[length]; // smooth 1
        double[] as2 = new double[length]; // smooth 2
        double sumA = 0;
        double sumA1 = 0;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            BasicData q = bdList[i];

            TsiResult r = new()
            {
                Date = q.Date
            };
            results.Add(r);

            // skip first period
            if (i == 0)
            {
                continue;
            }

            // price change
            c[i] = q.Value - bdList[i - 1].Value;
            a[i] = Math.Abs(c[i]);

            // smoothing
            if (i > lookbackPeriods)
            {
                // first smoothing
                cs1[i] = ((c[i] - cs1[i - 1]) * mult1) + cs1[i - 1];
                as1[i] = ((a[i] - as1[i - 1]) * mult1) + as1[i - 1];

                // second smoothing
                if (i + 1 > lookbackPeriods + smoothPeriods)
                {
                    cs2[i] = ((cs1[i] - cs2[i - 1]) * mult2) + cs2[i - 1];
                    as2[i] = ((as1[i] - as2[i - 1]) * mult2) + as2[i - 1];

                    double tsi = (as2[i] != 0) ? 100d * (cs2[i] / as2[i]) : double.NaN;
                    r.Tsi = tsi;

                    // signal line
                    if (signalPeriods > 0)
                    {
                        if (i >= lookbackPeriods + smoothPeriods + signalPeriods - 1)
                        {
                            r.Signal = ((tsi - results[i - 1].Signal) * multS)
                                     + results[i - 1].Signal;
                        }

                        // initialize signal
                        else
                        {
                            sumS += tsi;

                            if (i == lookbackPeriods + smoothPeriods + signalPeriods - 2)
                            {
                                r.Signal = sumS / signalPeriods;
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
                    if (i + 1 == lookbackPeriods + smoothPeriods)
                    {
                        cs2[i] = sumC1 / smoothPeriods;
                        as2[i] = sumA1 / smoothPeriods;

                        double tsi = (as2[i] != 0) ? 100 * cs2[i] / as2[i] : double.NaN;
                        r.Tsi = tsi;
                        sumS = tsi;
                    }
                }
            }

            // prepare first smoothing
            else
            {
                sumC += c[i];
                sumA += a[i];

                // initialize first smoothing
                if (i == lookbackPeriods)
                {
                    cs1[i] = sumC / lookbackPeriods;
                    as1[i] = sumA / lookbackPeriods;

                    sumC1 = cs1[i];
                    sumA1 = as1[i];
                }
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateTsi(
        int lookbackPeriods,
        int smoothPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for TSI.");
        }

        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smoothing periods must be greater than 0 for TSI.");
        }

        if (signalPeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than or equal to 0 for TSI.");
        }
    }
}
