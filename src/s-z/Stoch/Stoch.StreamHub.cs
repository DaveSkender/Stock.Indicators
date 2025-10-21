namespace Skender.Stock.Indicators;

// STOCHASTIC OSCILLATOR (STREAM HUB)

/// <summary>
/// Provides methods for creating Stochastic Oscillator hubs.
/// </summary>
public class StochHub
    : StreamHub<IQuote, StochResult>, IStoch
{
    #region constructors

    private readonly string hubName;
    private readonly RollingWindowMax<double> _highWindow;
    private readonly RollingWindowMin<double> _lowWindow;
    private readonly Queue<double> _rawKBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="StochHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The lookback period for the oscillator.</param>
    /// <param name="signalPeriods">The signal period for the oscillator.</param>
    /// <param name="smoothPeriods">The smoothing period for the oscillator.</param>
    internal StochHub(
        IStreamObservable<IQuote> provider,
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods) : this(provider, lookbackPeriods, signalPeriods, smoothPeriods, 3, 2, MaType.SMA)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StochHub"/> class with extended parameters.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The lookback period for the oscillator.</param>
    /// <param name="signalPeriods">The signal period for the oscillator.</param>
    /// <param name="smoothPeriods">The smoothing period for the oscillator.</param>
    /// <param name="kFactor">The K factor for the Stochastic calculation.</param>
    /// <param name="dFactor">The D factor for the Stochastic calculation.</param>
    /// <param name="movingAverageType">The type of moving average to use.</param>
    internal StochHub(
        IStreamObservable<IQuote> provider,
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

        // Initialize rolling windows for O(1) amortized max/min tracking
        _highWindow = new RollingWindowMax<double>(lookbackPeriods);
        _lowWindow = new RollingWindowMin<double>(lookbackPeriods);

        // Initialize buffer for raw K values (needed for SMA smoothing)
        _rawKBuffer = new Queue<double>(smoothPeriods);

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
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Add current high/low to rolling windows - O(1) amortized operation
        double high = (double)item.High;
        double low = (double)item.Low;
        double close = (double)item.Close;

        _highWindow.Add(high);
        _lowWindow.Add(low);

        // Calculate raw %K oscillator
        double rawK = double.NaN;
        if (i >= LookbackPeriods - 1)
        {
            // Check for NaN values in current quote
            bool isViable = !double.IsNaN(high) && !double.IsNaN(low) && !double.IsNaN(close);

            if (isViable)
            {
                // Use O(1) max/min retrieval from rolling windows
                double highHigh = _highWindow.Max;
                double lowLow = _lowWindow.Min;

                rawK = highHigh - lowLow != 0
                     ? 100 * (close - lowLow) / (highHigh - lowLow)
                     : 0;
            }
        }

        // Add raw K to buffer for smoothing calculation
        _rawKBuffer.Enqueue(rawK);
        if (_rawKBuffer.Count > SmoothPeriods)
        {
            _rawKBuffer.Dequeue();
        }

        // Calculate smoothed %K (oscillator) - matches StaticSeries logic
        double oscillator = double.NaN;
        if (SmoothPeriods <= 1)
        {
            oscillator = rawK;
        }
        else if (i >= SmoothPeriods)
        {
            switch (MovingAverageType)
            {
                case MaType.SMA:
                    {
                        // Use buffered raw K values for O(n) smoothing instead of O(n²) recalculation
                        double sum = 0;
                        foreach (double rawKValue in _rawKBuffer)
                        {
                            sum += rawKValue;
                        }

                        oscillator = sum / SmoothPeriods;
                        break;
                    }

                case MaType.SMMA:
                    {
                        // Get previous smoothed K from cache
                        double prevSmoothK;
                        if (i > SmoothPeriods && Cache.Count >= i && Cache[i - 1].Oscillator.HasValue)
                        {
                            prevSmoothK = Cache[i - 1].Oscillator!.Value;
                        }
                        else
                        {
                            // Re/initialize with current raw K
                            prevSmoothK = rawK;
                        }

                        oscillator = ((prevSmoothK * (SmoothPeriods - 1)) + rawK) / SmoothPeriods;
                        break;
                    }

                default:
                    throw new InvalidOperationException("Invalid Stochastic moving average type.");
            }
        }

        // Calculate %D signal line - matches StaticSeries logic
        double signal = double.NaN;
        if (SignalPeriods <= 1)
        {
            signal = oscillator;
        }
        else if (i >= SignalPeriods)
        {
            switch (MovingAverageType)
            {
                case MaType.SMA:
                    {
                        double sum = 0;
                        // Get smoothed K values from cache for the signal window
                        for (int p = i - SignalPeriods + 1; p <= i; p++)
                        {
                            double smoothKAtP = double.NaN;
                            if (p < i && Cache.Count > p && Cache[p].Oscillator.HasValue)
                            {
                                // Get from cache for previous positions
                                smoothKAtP = Cache[p].Oscillator!.Value;
                            }
                            else if (p == i)
                            {
                                // Use current oscillator for position i
                                smoothKAtP = oscillator;
                            }

                            sum += smoothKAtP;
                        }

                        signal = sum / SignalPeriods;
                        break;
                    }

                case MaType.SMMA:
                    {
                        // Get previous signal from cache
                        double prevSignal;
                        if (i > SignalPeriods && Cache.Count >= i && Cache[i - 1].Signal.HasValue)
                        {
                            prevSignal = Cache[i - 1].Signal!.Value;
                        }
                        else
                        {
                            // Re/initialize with current oscillator
                            prevSignal = oscillator;
                        }

                        signal = ((prevSignal * (SignalPeriods - 1)) + oscillator) / SignalPeriods;
                        break;
                    }

                default:
                    throw new InvalidOperationException("Invalid Stochastic moving average type.");
            }
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


public static partial class Stoch
{
    /// <summary>
    /// Converts the quote provider to a Stochastic Oscillator hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The lookback period for the oscillator.</param>
    /// <param name="signalPeriods">The signal period for the oscillator.</param>
    /// <param name="smoothPeriods">The smoothing period for the oscillator.</param>
    /// <returns>A Stochastic Oscillator hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static StochHub ToStochHub(
        this IStreamObservable<IQuote> quoteProvider,
        int lookbackPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 3)
             => new(quoteProvider, lookbackPeriods, signalPeriods, smoothPeriods);

    /// <summary>
    /// Converts the quote provider to a Stochastic Oscillator hub with extended parameters.
    /// </summary>
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
    public static StochHub ToStoch(
        this IStreamObservable<IQuote> quoteProvider,
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        double kFactor,
        double dFactor,
        MaType movingAverageType)
             => new(quoteProvider, lookbackPeriods, signalPeriods, smoothPeriods, kFactor, dFactor, movingAverageType);

    /// <summary>
    /// Creates a Stoch hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <param name="signalPeriods">Parameter for the calculation.</param>
    /// <param name="smoothPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="StochHub"/>.</returns>
    public static StochHub ToStochHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 3)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStochHub(lookbackPeriods, signalPeriods, smoothPeriods);
    }

}
