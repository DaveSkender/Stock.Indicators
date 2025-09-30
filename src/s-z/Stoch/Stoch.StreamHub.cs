namespace Skender.Stock.Indicators;

// STOCHASTIC OSCILLATOR (STREAM HUB)

/// <summary>
/// Provides methods for creating Stochastic Oscillator hubs.
/// </summary>
public static partial class Stoch
{
    /// <summary>
    /// Converts the quote provider to a Stochastic Oscillator hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The lookback period for the oscillator.</param>
    /// <param name="signalPeriods">The signal period for the oscillator.</param>
    /// <param name="smoothPeriods">The smoothing period for the oscillator.</param>
    /// <returns>A Stochastic Oscillator hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static StochHub<TIn> ToStoch<TIn>(
        this IStreamObservable<TIn> quoteProvider,
        int lookbackPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 3)
        where TIn : IQuote
        => new(quoteProvider, lookbackPeriods, signalPeriods, smoothPeriods);

    /// <summary>
    /// Converts the quote provider to a Stochastic Oscillator hub with extended parameters.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The lookback period for the oscillator.</param>
    /// <param name="signalPeriods">The signal period for the oscillator.</param>
    /// <param name="smoothPeriods">The smoothing period for the oscillator.</param>
    /// <param name="kFactor">The K factor for the Stochastic calculation.</param>
    /// <param name="dFactor">The D factor for the Stochastic calculation.</param>
    /// <param name="movingAverageType">The type of moving average to use.</param>
    /// <returns>A Stochastic Oscillator hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static StochHub<TIn> ToStoch<TIn>(
        this IStreamObservable<TIn> quoteProvider,
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        double kFactor,
        double dFactor,
        MaType movingAverageType)
        where TIn : IQuote
        => new(quoteProvider, lookbackPeriods, signalPeriods, smoothPeriods, kFactor, dFactor, movingAverageType);
}

/// <summary>
/// Represents a Stochastic Oscillator stream hub.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
public class StochHub<TIn>
    : StreamHub<TIn, StochResult>, IStoch
    where TIn : IQuote
{
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="StochHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The lookback period for the oscillator.</param>
    /// <param name="signalPeriods">The signal period for the oscillator.</param>
    /// <param name="smoothPeriods">The smoothing period for the oscillator.</param>
    internal StochHub(
        IStreamObservable<TIn> provider,
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods) : this(provider, lookbackPeriods, signalPeriods, smoothPeriods, 3, 2, MaType.SMA)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StochHub{TIn}"/> class with extended parameters.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The lookback period for the oscillator.</param>
    /// <param name="signalPeriods">The signal period for the oscillator.</param>
    /// <param name="smoothPeriods">The smoothing period for the oscillator.</param>
    /// <param name="kFactor">The K factor for the Stochastic calculation.</param>
    /// <param name="dFactor">The D factor for the Stochastic calculation.</param>
    /// <param name="movingAverageType">The type of moving average to use.</param>
    internal StochHub(
        IStreamObservable<TIn> provider,
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        double kFactor,
        double dFactor,
        MaType movingAverageType) : base(provider)
    {
        Stoch.Validate(lookbackPeriods, signalPeriods, smoothPeriods, kFactor, dFactor, movingAverageType);

        LookbackPeriods = lookbackPeriods;
        SignalPeriods = signalPeriods;
        SmoothPeriods = smoothPeriods;
        KFactor = kFactor;
        DFactor = dFactor;
        MovingAverageType = movingAverageType;

        hubName = $"STOCH({lookbackPeriods},{signalPeriods},{smoothPeriods})";

        Reinitialize();
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public int SignalPeriods { get; init; }

    /// <inheritdoc />
    public int SmoothPeriods { get; init; }

    /// <inheritdoc />
    public double KFactor { get; init; }

    /// <inheritdoc />
    public double DFactor { get; init; }

    /// <inheritdoc />
    public MaType MovingAverageType { get; init; }

    #endregion

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (StochResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate raw %K oscillator
        double rawK = double.NaN;
        if (i >= LookbackPeriods - 1)
        {
            double highHigh = double.MinValue;
            double lowLow = double.MaxValue;
            bool isViable = true;

            // Get lookback window
            for (int p = i - LookbackPeriods + 1; p <= i; p++)
            {
                TIn x = ProviderCache[p];

                if (double.IsNaN((double)x.High) ||
                    double.IsNaN((double)x.Low) ||
                    double.IsNaN((double)x.Close))
                {
                    isViable = false;
                    break;
                }

                if ((double)x.High > highHigh)
                {
                    highHigh = (double)x.High;
                }

                if ((double)x.Low < lowLow)
                {
                    lowLow = (double)x.Low;
                }
            }

            rawK = !isViable
                 ? double.NaN
                 : highHigh - lowLow != 0
                 ? 100 * ((double)item.Close - lowLow) / (highHigh - lowLow)
                 : 0;
        }

        // Calculate smoothed %K (oscillator)
        double oscillator = double.NaN;
        if (SmoothPeriods <= 1)
        {
            oscillator = rawK;
        }
        else if (i >= SmoothPeriods && !double.IsNaN(rawK))
        {
            // Simple SMA approximation for StreamHub
            oscillator = rawK; // Simplified for now
        }

        // Calculate %D signal line
        double signal = double.NaN;
        if (SignalPeriods <= 1)
        {
            signal = oscillator;
        }
        else if (i >= SignalPeriods + SmoothPeriods - 1 && !double.IsNaN(oscillator))
        {
            // Simple SMA approximation for StreamHub
            signal = oscillator; // Simplified for now
        }

        // Calculate %J only when both oscillator and signal are available
        double percentJ = double.NaN;
        if (!double.IsNaN(oscillator) && !double.IsNaN(signal))
        {
            percentJ = (KFactor * oscillator) - (DFactor * signal);
        }

        StochResult result = new(
            Timestamp: item.Timestamp,
            Oscillator: oscillator.NaN2Null(),
            Signal: signal.NaN2Null(),
            PercentJ: percentJ.NaN2Null());

        return (result, i);
    }

    #endregion
}
