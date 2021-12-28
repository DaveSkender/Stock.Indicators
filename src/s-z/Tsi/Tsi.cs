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
        List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.Close);

        // check parameter arguments
        ValidateTsi(quotes, lookbackPeriods, smoothPeriods, signalPeriods);

        // initialize
        int size = bdList.Count;
        double mult1 = 2d / (lookbackPeriods + 1);
        double mult2 = 2d / (smoothPeriods + 1);
        double multS = 2d / (signalPeriods + 1);
        double? sumS = 0;

        List<TsiResult> results = new(size);

        double[] c = new double[size]; // price change
        double[] cs1 = new double[size]; // smooth 1
        double[] cs2 = new double[size]; // smooth 2
        double sumC = 0;
        double sumC1 = 0;

        double[] a = new double[size]; // abs of price change
        double[] as1 = new double[size]; // smooth 1
        double[] as2 = new double[size]; // smooth 2
        double sumA = 0;
        double sumA1 = 0;

        // roll through quotes
        for (int i = 0; i < size; i++)
        {
            BasicD q = bdList[i];
            int index = i + 1;

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
            if (index > lookbackPeriods + 1)
            {
                // first smoothing
                cs1[i] = ((c[i] - cs1[i - 1]) * mult1) + cs1[i - 1];
                as1[i] = ((a[i] - as1[i - 1]) * mult1) + as1[i - 1];

                // second smoothing
                if (index > lookbackPeriods + smoothPeriods)
                {
                    cs2[i] = ((cs1[i] - cs2[i - 1]) * mult2) + cs2[i - 1];
                    as2[i] = ((as1[i] - as2[i - 1]) * mult2) + as2[i - 1];

                    r.Tsi = (as2[i] != 0) ? 100d * (cs2[i] / as2[i]) : null;

                    // signal line
                    if (signalPeriods > 0)
                    {
                        if (index >= lookbackPeriods + smoothPeriods + signalPeriods)
                        {
                            r.Signal = ((r.Tsi - results[i - 1].Signal) * multS)
                                     + results[i - 1].Signal;
                        }

                        // initialize signal
                        else
                        {
                            sumS += r.Tsi;

                            if (index == lookbackPeriods + smoothPeriods + signalPeriods - 1)
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
                    if (index == lookbackPeriods + smoothPeriods)
                    {
                        cs2[i] = sumC1 / smoothPeriods;
                        as2[i] = sumA1 / smoothPeriods;

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
                if (index == lookbackPeriods + 1)
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

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<TsiResult> RemoveWarmupPeriods(
        this IEnumerable<TsiResult> results)
    {
        int nm = results
            .ToList()
            .FindIndex(x => x.Tsi != null) + 1;

        return results.Remove(nm + 250);
    }

    // parameter validation
    private static void ValidateTsi<TQuote>(
        IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int smoothPeriods,
        int signalPeriods)
        where TQuote : IQuote
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

        // check quotes
        int qtyHistory = quotes.Count();
        int minHistory = lookbackPeriods + smoothPeriods + 100;
        if (qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for TSI.  " +
                string.Format(
                    EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                + "Since this uses a double smoothing technique, for an N+M period of {2}, "
                + "we recommend you use at least {3} data points prior to the intended "
                + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods + smoothPeriods,
                    lookbackPeriods + smoothPeriods + 250);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
