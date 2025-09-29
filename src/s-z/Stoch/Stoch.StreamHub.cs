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

        if (i < LookbackPeriods + SmoothPeriods - 2)
        {
            return (new StochResult(item.Timestamp, null, null, null), i);
        }

        // Get data for calculation window
        List<TIn> window = ProviderCache
            .Skip(Math.Max(0, i - LookbackPeriods + 1))
            .Take(Math.Min(LookbackPeriods, i + 1))
            .ToList();

        if (window.Count < LookbackPeriods)
        {
            return (new StochResult(item.Timestamp, null, null, null), i);
        }

        // Calculate %K oscillator
        double high = window.Max(x => (double)x.High);
        double low = window.Min(x => (double)x.Low);
        double close = (double)item.Close;

        double? rawK = null;
        if (high - low != 0)
        {
            rawK = 100.0 * (close - low) / (high - low);
        }
        else
        {
            rawK = 0;
        }

        // For simplicity in streaming, we'll implement basic SMA smoothing
        // More complex SMMA would require additional state management
        double? smoothK = rawK;
        double? signal = null;

        if (i >= LookbackPeriods + SmoothPeriods - 2)
        {
            // Get smoothed %K values for signal calculation
            List<StochResult> recentResults = Cache
                .Skip(Math.Max(0, Cache.Count - SignalPeriods))
                .Where(x => x.Oscillator.HasValue)
                .ToList();

            if (recentResults.Count >= SignalPeriods - 1)
            {
                List<double> kValues = recentResults
                    .Select(x => x.Oscillator!.Value)
                    .ToList();
                kValues.Add(smoothK!.Value);

                signal = kValues.TakeLast(SignalPeriods).Average();
            }
        }

        // Calculate %J
        double? percentJ = null;
        if (smoothK.HasValue && signal.HasValue)
        {
            percentJ = (KFactor * smoothK.Value) - (DFactor * signal.Value);
        }

        StochResult result = new(
            Timestamp: item.Timestamp,
            Oscillator: smoothK,
            Signal: signal,
            PercentJ: percentJ);

        return (result, i);
    }

    #endregion
}
