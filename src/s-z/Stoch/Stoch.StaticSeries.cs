#pragma warning disable IDE0010 // Missing cases in switch expression

/// <summary>
/// Provides methods for calculating the Stochastic Oscillator.
/// </summary>
public static partial class Stoch
{
    /// <summary>
    /// Calculates the Stochastic Oscillator for a series of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="lookbackPeriods">The lookback period for the oscillator.</param>
    /// <param name="signalPeriods">The signal period for the oscillator.</param>
    /// <param name="smoothPeriods">The smoothing period for the oscillator.</param>
    /// <returns>A list of StochResult containing the oscillator values.</returns>
    public static IReadOnlyList<StochResult> ToStoch<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 3)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcStoch(
                lookbackPeriods,
                signalPeriods,
                smoothPeriods, 3, 2, MaType.SMA);

    /// <summary>
    /// Calculates the Stochastic Oscillator for a series of quotes with specified factors and moving average type.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="lookbackPeriods">The lookback period for the oscillator.</param>
    /// <param name="signalPeriods">The signal period for the oscillator.</param>
    /// <param name="smoothPeriods">The smoothing period for the oscillator.</param>
    /// <param name="kFactor">The factor for the %K line.</param>
    /// <param name="dFactor">The factor for the %D line.</param>
    /// <param name="movingAverageType">The type of moving average to use.</param>
    /// <returns>A list of StochResult containing the oscillator values.</returns>
    public static IReadOnlyList<StochResult> ToStoch<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        double kFactor,
        double dFactor,
        MaType movingAverageType)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcStoch(
                lookbackPeriods,
                signalPeriods,
                smoothPeriods,
                kFactor,
                dFactor,
                movingAverageType);

    /// <summary>
    /// Calculates the Stochastic Oscillator for a series of quotes.
    /// </summary>
    /// <param name="source">The list of quotes.</param>
    /// <param name="lookbackPeriods">The lookback period for the oscillator.</param>
    /// <param name="signalPeriods">The signal period for the oscillator.</param>
    /// <param name="smoothPeriods">The smoothing period for the oscillator.</param>
    /// <param name="kFactor">The factor for the %K line.</param>
    /// <param name="dFactor">The factor for the %D line.</param>
    /// <param name="movingAverageType">The type of moving average to use.</param>
    /// <returns>A list of StochResult containing the oscillator values.</returns>
    internal static List<StochResult> CalcStoch(
        this List<QuoteD> source,
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        double kFactor,
        double dFactor,
        MaType movingAverageType)
    {
        // check parameter arguments
        Validate(
            lookbackPeriods, signalPeriods, smoothPeriods,
            kFactor, dFactor, movingAverageType);

        // initialize
        int length = source.Count;
        List<StochResult> results = new(length);

        double[] o = new double[length]; // %K oscillator (initial)
        double[] k = new double[length]; // %K oscillator (final)

        double prevK = double.NaN;
        double prevD = double.NaN;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];

            // initial %K oscillator
            if (i >= lookbackPeriods - 1)
            {
                double highHigh = double.MinValue;
                double lowLow = double.MaxValue;
                bool isViable = true;

                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    QuoteD x = source[p];

                    if (double.IsNaN(x.High)
                     || double.IsNaN(x.Low)
                     || double.IsNaN(x.Close))
                    {
                        isViable = false;
                        break;
                    }

                    if (x.High > highHigh)
                    {
                        highHigh = x.High;
                    }

                    if (x.Low < lowLow)
                    {
                        lowLow = x.Low;
                    }
                }

                o[i] = !isViable
                     ? double.NaN
                     : highHigh - lowLow != 0
                     ? 100 * (q.Close - lowLow) / (highHigh - lowLow)
                     : 0;
            }
            else
            {
                o[i] = double.NaN;
            }

            // final %K oscillator, keep original
            if (smoothPeriods <= 1)
            {
                k[i] = o[i];
            }

            // final %K oscillator, if smoothed
            else if (i >= smoothPeriods)
            {
                k[i] = double.NaN;

                switch (movingAverageType)
                {
                    // SMA case
                    case MaType.SMA:
                        {
                            double sum = 0;
                            for (int p = i - smoothPeriods + 1; p <= i; p++)
                            {
                                sum += o[p];
                            }

                            k[i] = sum / smoothPeriods;
                            break;
                        }

                    // SMMA case
                    case MaType.SMMA:
                        {
                            // re/initialize
                            if (double.IsNaN(prevK))
                            {
                                prevK = o[i];
                            }

                            k[i] = ((prevK * (smoothPeriods - 1)) + o[i]) / smoothPeriods;
                            prevK = k[i];
                            break;
                        }

                    default:
                        throw new InvalidOperationException(
                            "Invalid Stochastic moving average type.");
                }
            }
            else
            {
                k[i] = double.NaN;
            }

            double oscillator = k[i];
            double signal;


            // %D signal line
            if (signalPeriods <= 1)
            {
                signal = oscillator;
            }
            else if (i >= signalPeriods)
            {
                switch (movingAverageType)
                {
                    // SMA case
                    // TODO: || double.IsNaN(prevD) to re/initialize SMMA?
                    case MaType.SMA:
                        {
                            double sum = 0;
                            for (int p = i - signalPeriods + 1; p <= i; p++)
                            {
                                sum += k[p];
                            }

                            signal = sum / signalPeriods;
                            break;
                        }

                    // SMMA case
                    case MaType.SMMA:
                        {
                            // re/initialize
                            if (double.IsNaN(prevD))
                            {
                                prevD = k[i];
                            }

                            double d = ((prevD * (signalPeriods - 1)) + k[i]) / signalPeriods;
                            signal = d;
                            prevD = d;
                            break;
                        }

                    default:
                        throw new InvalidOperationException("Invalid Stochastic moving average type.");
                }
            }
            else
            {
                signal = double.NaN;
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                Oscillator: oscillator.NaN2Null(),
                Signal: signal.NaN2Null(),
                PercentJ: ((kFactor * oscillator) - (dFactor * signal)).NaN2Null()));
        }

        return results;
    }
}
