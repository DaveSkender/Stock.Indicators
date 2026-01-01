namespace Skender.Stock.Indicators;

/// <summary>
/// Stochastic Momentum Index (SMI) indicator.
/// </summary>
public static partial class Smi
{
    /// <summary>
    /// Converts a list of quotes to SMI results.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback window.</param>
    /// <param name="firstSmoothPeriods">The number of periods for the first smoothing.</param>
    /// <param name="secondSmoothPeriods">The number of periods for the second smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line smoothing.</param>
    /// <returns>A list of SMI results.</returns>
    public static IReadOnlyList<SmiResult> ToSmi(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 13,
        int firstSmoothPeriods = 25,
        int secondSmoothPeriods = 2,
        int signalPeriods = 3)
        => quotes
            .ToQuoteDList()
            .CalcSmi(
                lookbackPeriods,
                firstSmoothPeriods,
                secondSmoothPeriods,
                signalPeriods);

    /// <summary>
    /// Calculates the SMI for a list of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback window.</param>
    /// <param name="firstSmoothPeriods">The number of periods for the first smoothing.</param>
    /// <param name="secondSmoothPeriods">The number of periods for the second smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line smoothing.</param>
    /// <returns>A list of SMI results.</returns>
    private static List<SmiResult> CalcSmi(
        this List<QuoteD> quotes,
        int lookbackPeriods,
        int firstSmoothPeriods,
        int secondSmoothPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        Validate(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        // initialize
        int length = quotes.Count;
        List<SmiResult> results = new(length);

        double k1 = 2d / (firstSmoothPeriods + 1);
        double k2 = 2d / (secondSmoothPeriods + 1);
        double kS = 2d / (signalPeriods + 1);

        double lastSmEma1 = double.NaN;
        double lastSmEma2 = double.NaN;
        double lastHlEma1 = double.NaN;
        double lastHlEma2 = double.NaN;
        double lastSignal = double.NaN;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotes[i];

            double smi;
            double signal;

            if (i >= lookbackPeriods - 1)
            {
                double hH = double.MinValue;
                double lL = double.MaxValue;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    QuoteD x = quotes[p];

                    if (x.High > hH)
                    {
                        hH = x.High;
                    }

                    if (x.Low < lL)
                    {
                        lL = x.Low;
                    }
                }

                double sm = q.Close - (0.5d * (hH + lL));
                double hl = hH - lL;

                // Initialize last EMA values when no prior state exists
                if (double.IsNaN(lastSmEma1))
                {
                    lastSmEma1 = sm;
                    lastSmEma2 = lastSmEma1;
                    lastHlEma1 = hl;
                    lastHlEma2 = lastHlEma1;
                }

                // first smoothing
                double smEma1 = lastSmEma1 + (k1 * (sm - lastSmEma1));
                double hlEma1 = lastHlEma1 + (k1 * (hl - lastHlEma1));

                // second smoothing
                double smEma2 = lastSmEma2 + (k2 * (smEma1 - lastSmEma2));
                double hlEma2 = lastHlEma2 + (k2 * (hlEma1 - lastHlEma2));

                // stochastic momentum index
                smi = 100 * (smEma2 / (0.5 * hlEma2));

                // Initialize signal line when no prior state exists
                if (double.IsNaN(lastSignal))
                {
                    lastSignal = smi;
                }

                // signal line
                signal = lastSignal + (kS * (smi - lastSignal));

                // carryover values
                lastSmEma1 = smEma1;
                lastSmEma2 = smEma2;
                lastHlEma1 = hlEma1;
                lastHlEma2 = hlEma2;
                lastSignal = signal;
            }
            else
            {
                smi = double.NaN;
                signal = double.NaN;
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                Smi: smi.NaN2Null(),
                Signal: signal.NaN2Null()));
        }

        return results;
    }
}
