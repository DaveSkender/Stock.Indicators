namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Stochastic Momentum Index (SMI) indicator.
/// </summary>
public static partial class Smi
{
    /// <summary>
    /// Converts a list of quotes to SMI results.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback window.</param>
    /// <param name="firstSmoothPeriods">The number of periods for the first smoothing.</param>
    /// <param name="secondSmoothPeriods">The number of periods for the second smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line smoothing.</param>
    /// <returns>A list of SMI results.</returns>
    [Series("SMI", "Stochastic Momentum Index", Category.Oscillator, ChartType.Oscillator)]
    public static IReadOnlyList<SmiResult> ToSmi<TQuote>(
        this IReadOnlyList<TQuote> quotes,

        [Param("Lookback Periods", 1, 300, 13)]
        int lookbackPeriods = 13,

        [Param("First Smooth Periods", 1, 300, 25)]
        int firstSmoothPeriods = 25,

        [Param("Second Smooth Periods", 1, 50, 2)]
        int secondSmoothPeriods = 2,

        [Param("Signal Periods", 1, 50, 3)]
        int signalPeriods = 3)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcSmi(
                lookbackPeriods,
                firstSmoothPeriods,
                secondSmoothPeriods,
                signalPeriods);

    /// <summary>
    /// Calculates the SMI for a list of quotes.
    /// </summary>
    /// <param name="source">The list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback window.</param>
    /// <param name="firstSmoothPeriods">The number of periods for the first smoothing.</param>
    /// <param name="secondSmoothPeriods">The number of periods for the second smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line smoothing.</param>
    /// <returns>A list of SMI results.</returns>
    private static List<SmiResult> CalcSmi(
        this List<QuoteD> source,
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
        int length = source.Count;
        List<SmiResult> results = new(length);

        double k1 = 2d / (firstSmoothPeriods + 1);
        double k2 = 2d / (secondSmoothPeriods + 1);
        double kS = 2d / (signalPeriods + 1);

        double lastSmEma1 = 0;
        double lastSmEma2 = 0;
        double lastHlEma1 = 0;
        double lastHlEma2 = 0;
        double lastSignal = 0;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];

            double smi;
            double signal;

            if (i >= lookbackPeriods - 1)
            {
                double hH = double.MinValue;
                double lL = double.MaxValue;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    QuoteD x = source[p];

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

                // initialize last EMA values
                // TODO: update healing, without requiring specific indexing
                if (i == lookbackPeriods - 1)
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

                // initialize signal line
                // TODO: update healing, without requiring specific indexing
                if (i == lookbackPeriods - 1)
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
