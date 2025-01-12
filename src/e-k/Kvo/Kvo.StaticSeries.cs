/// <summary>
/// Provides methods for calculating the Klinger Volume Oscillator (KVO) for a series of quotes.
/// </summary>
public static partial class Kvo
{
    /// <summary>
    /// Converts a list of quotes to KVO (Klinger Volume Oscillator) results.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quotes, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">The list of quotes to transform.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA. Default is 34.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA. Default is 55.</param>
    /// <param name="signalPeriods">The number of periods for the signal line. Default is 13.</param>
    /// <returns>A list of KVO results.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are out of their valid range.</exception>
    public static IReadOnlyList<KvoResult> ToKvo<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int fastPeriods = 34,
        int slowPeriods = 55,
        int signalPeriods = 13)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcKvo(fastPeriods, slowPeriods, signalPeriods);

    /// <summary>
    /// Calculates the KVO (Klinger Volume Oscillator) for a list of quotes.
    /// </summary>
    /// <param name="source">The list of quotes to process.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <returns>A list of KVO results.</returns>
    private static List<KvoResult> CalcKvo(
        this List<QuoteD> source,
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        Validate(fastPeriods, slowPeriods, signalPeriods);

        // initialize
        int length = source.Count;
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
            QuoteD q = source[i];

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

            // TODO: update healing, without requiring specific indexing
            else if (i == fastPeriods + 1)
            {
                double sum = 0;
                for (int p = 2; p <= i; p++)
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

            // TODO: update healing, without requiring specific indexing
            else if (i == slowPeriods + 1)
            {
                double sum = 0;
                for (int p = 2; p <= i; p++)
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

                // TODO: update healing, without requiring specific indexing
                else if (i == slowPeriods + signalPeriods)
                {
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
