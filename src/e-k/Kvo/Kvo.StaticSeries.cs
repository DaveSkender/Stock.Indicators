namespace Skender.Stock.Indicators;

/// <summary>
/// Klinger Volume Oscillator (KVO) for a series of quotes indicator.
/// </summary>
public static partial class Kvo
{
    /// <summary>
    /// Converts a list of quotes to KVO (Klinger Volume Oscillator) results.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <returns>A list of KVO results.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are out of their valid range.</exception>
    public static IReadOnlyList<KvoResult> ToKvo(
        this IReadOnlyList<IQuote> quotes,
        int fastPeriods = 34,
        int slowPeriods = 55,
        int signalPeriods = 13)
        => quotes
            .ToQuoteDList()
            .CalcKvo(fastPeriods, slowPeriods, signalPeriods);

    /// <summary>
    /// Calculates the KVO (Klinger Volume Oscillator) for a list of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <returns>A list of KVO results.</returns>
    private static List<KvoResult> CalcKvo(
        this List<QuoteD> quotes,
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        Validate(fastPeriods, slowPeriods, signalPeriods);

        // initialize
        int length = quotes.Count;
        List<KvoResult> results = new(length);

        double[] t = new double[length];          // trend direction
        double[] hlc = new double[length];        // trend basis
        double[] dm = new double[length];         // daily measurement
        double[] cm = new double[length];         // cumulative measurement
        double[] vf = new double[length];         // volume force (VF)
        double[] vfFastEma = new double[length];  // EMA of VF (short-term)
        double[] vfSlowEma = new double[length];  // EMA of VP (long-term)

        // EMA multipliers
        double kFast = 2d / (fastPeriods + 1);
        double kSlow = 2d / (slowPeriods + 1);
        double kSignal = 2d / (signalPeriods + 1);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotes[i];

            double? kvo = null;
            double? sig = null;

            // trend basis comparator
            hlc[i] = q.High + q.Low + q.Close;

            // daily measurement
            dm[i] = q.High - q.Low;

            if (i <= 0)
            {
                results.Add(new(q.Timestamp));
                continue;
            }

            // trend direction
            t[i] = hlc[i] > hlc[i - 1] ? 1 : -1;

            if (i <= 1)
            {
                cm[i] = 0;
                results.Add(new(q.Timestamp));
                continue;
            }

            // cumulative measurement
            cm[i] = t[i] == t[i - 1] ?
                    cm[i - 1] + dm[i] : dm[i - 1] + dm[i];

            // volume force (VF)
            vf[i] = dm[i] == cm[i] || q.Volume == 0 ? 0
                : dm[i] == 0 ? q.Volume * 2d * t[i] * 100d
                : cm[i] != 0 ? q.Volume * Math.Abs(2d * ((dm[i] / cm[i]) - 1)) * t[i] * 100d
                : vf[i - 1];

            // fast-period EMA of VF
            if (i > fastPeriods + 1)
            {
                vfFastEma[i] = (vf[i] * kFast) + (vfFastEma[i - 1] * (1 - kFast));
            }
            else if (i == fastPeriods + 1)
            {
                // initialize fast EMA with average of most recent fastPeriods values
                double sum = 0;
                for (int p = i - fastPeriods + 1; p <= i; p++)
                {
                    sum += vf[p];
                }

                vfFastEma[i] = sum / fastPeriods;
            }

            // slow-period EMA of VF
            if (i > slowPeriods + 1)
            {
                vfSlowEma[i] = (vf[i] * kSlow) + (vfSlowEma[i - 1] * (1 - kSlow));
            }
            else if (i == slowPeriods + 1)
            {
                // initialize slow EMA with average of most recent slowPeriods values
                double sum = 0;
                for (int p = i - slowPeriods + 1; p <= i; p++)
                {
                    sum += vf[p];
                }

                vfSlowEma[i] = sum / slowPeriods;
            }

            // Klinger Oscillator
            if (i >= slowPeriods + 1)
            {
                kvo = vfFastEma[i] - vfSlowEma[i];

                // Signal
                if (i > slowPeriods + signalPeriods)
                {
                    sig = (kvo * kSignal)
                        + (results[i - 1].Signal * (1 - kSignal));
                }
                else if (i == slowPeriods + signalPeriods)
                {
                    // initialize signal
                    double? sum = kvo;
                    for (int p = slowPeriods + 1; p < i; p++)
                    {
                        sum += results[p].Oscillator;
                    }

                    sig = sum / signalPeriods;
                }
            }

            results.Add(new KvoResult(
                Timestamp: q.Timestamp,
                Oscillator: kvo,
                Signal: sig));
        }

        return results;
    }
}
