namespace Skender.Stock.Indicators;

// TILLSON T3 MOVING AVERAGE (SERIES)
public static partial class Indicator
{
    internal static List<T3Result> CalcT3(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        double volumeFactor)
    {
        // check parameter arguments
        ValidateT3(lookbackPeriods, volumeFactor);

        // initialize
        int length = tpList.Count;
        List<T3Result> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double a = volumeFactor;
        double c1 = -a * a * a;
        double c2 = (3 * a * a) + (3 * a * a * a);
        double c3 = (-6 * a * a) - (3 * a) - (3 * a * a * a);
        double c4 = 1 + (3 * a) + (a * a * a) + (3 * a * a);

        double? e1 = 0, e2 = 0, e3 = 0, e4 = 0, e5 = 0, e6 = 0;
        double? sum1 = 0, sum2 = 0, sum3 = 0, sum4 = 0, sum5 = 0, sum6 = 0;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];
            T3Result r = new(date);
            results.Add(r);

            // first smoothing
            if (i > lookbackPeriods - 1)
            {
                e1 += k * (value - e1);

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
                                    r.T3 = ((c1 * e6) + (c2 * e5) + (c3 * e4) + (c4 * e3)).NaN2Null();
                                }

                                // sixth warmup
                                else
                                {
                                    sum6 += e5;

                                    if (i == 6 * (lookbackPeriods - 1))
                                    {
                                        e6 = sum6 / lookbackPeriods;

                                        // initial T3 moving average
                                        r.T3 = ((c1 * e6) + (c2 * e5) + (c3 * e4) + (c4 * e3)).NaN2Null();
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
                sum1 += value;

                if (i == lookbackPeriods - 1)
                {
                    sum2 = e1 = sum1 / lookbackPeriods;
                }
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateT3(
        int lookbackPeriods,
        double volumeFactor)
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
    }
}
