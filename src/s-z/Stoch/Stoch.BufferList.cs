namespace Skender.Stock.Indicators;

/// <summary>
/// Stochastic Oscillator from incremental quote values.
/// </summary>
public class StochList : BufferList<StochResult>, IIncrementFromQuote, IStoch
{
    private readonly Queue<(double High, double Low, double Close)> _hlcBuffer;
    private readonly Queue<double> _rawKBuffer;
    private readonly Queue<double> _smoothKBuffer;

    private double _prevSmoothK = double.NaN;
    private double _prevSignal = double.NaN;

    /// <summary>
    /// Initializes a new instance of the <see cref="StochList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the oscillator calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing the oscillator.</param>
    /// <param name="kFactor">The K factor for the Stochastic calculation.</param>
    /// <param name="dFactor">The D factor for the Stochastic calculation.</param>
    /// <param name="movingAverageType">The type of moving average to use.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="movingAverageType"/> is invalid.</exception>
    public StochList(
        int lookbackPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 3,
        double kFactor = 3,
        double dFactor = 2,
        MaType movingAverageType = MaType.SMA)
    {
        Stoch.Validate(lookbackPeriods, signalPeriods, smoothPeriods, kFactor, dFactor, movingAverageType);

        LookbackPeriods = lookbackPeriods;
        SignalPeriods = signalPeriods;
        SmoothPeriods = smoothPeriods;
        KFactor = kFactor;
        DFactor = dFactor;
        MovingAverageType = movingAverageType;

        _hlcBuffer = new Queue<(double, double, double)>(lookbackPeriods);
        _rawKBuffer = new Queue<double>(smoothPeriods);
        _smoothKBuffer = new Queue<double>(signalPeriods);

        Name = $"STOCH({14}, {3}, {3}, {3}, {2}, {MaType.SMA})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StochList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the oscillator calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing the oscillator.</param>
    /// <param name="kFactor">The K factor for the Stochastic calculation.</param>
    /// <param name="dFactor">The D factor for the Stochastic calculation.</param>
    /// <param name="movingAverageType">The type of moving average to use.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public StochList(
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        double kFactor,
        double dFactor,
        MaType movingAverageType,
        IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods, signalPeriods, smoothPeriods, kFactor, dFactor, movingAverageType) => Add(quotes);

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

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, (double)quote.High, (double)quote.Low, (double)quote.Close);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i]);
        }
    }

    /// <summary>
    /// Adds a new quote data point for Stochastic calculation.
    /// </summary>
    /// <param name="timestamp">The timestamp of the data point.</param>
    /// <param name="high">The high price.</param>
    /// <param name="low">The low price.</param>
    /// <param name="close">The close price.</param>
    /// <exception cref="InvalidOperationException">Thrown when the operation is invalid for the current state</exception>
    public void Add(DateTime timestamp, double high, double low, double close)
    {
        // Update rolling buffer using BufferListUtilities with consolidated tuple
        _hlcBuffer.Update(LookbackPeriods, (high, low, close));

        // Calculate raw %K oscillator when we have enough data
        double rawK = double.NaN;
        if (_hlcBuffer.Count == LookbackPeriods)
        {
            double highHigh = double.MinValue;
            double lowLow = double.MaxValue;

            foreach ((double High, double Low, double Close) in _hlcBuffer)
            {
                if (High > highHigh)
                {
                    highHigh = High;
                }

                if (Low < lowLow)
                {
                    lowLow = Low;
                }
            }

            // Boundary detection to avoid floating-point precision errors at 0 and 100
            if (highHigh == lowLow)
            {
                rawK = 0d;
            }
            else if (close >= highHigh)
            {
                // Exact 100 when close equals or exceeds highHigh
                rawK = 100d;
            }
            else if (close <= lowLow)
            {
                // Exact 0 when close equals or falls below lowLow
                rawK = 0d;
            }
            else
            {
                rawK = 100d * (close - lowLow) / (highHigh - lowLow);
            }
        }

        // Calculate smoothed %K (final oscillator) - logic matches StaticSeries
        double smoothK = double.NaN;
        if (SmoothPeriods <= 1)
        {
            // No smoothing needed - keep original
            smoothK = rawK;
        }
        else if (Count >= SmoothPeriods && !double.IsNaN(rawK)) // This matches StaticSeries: i >= smoothPeriods
        {
            // Update raw K buffer for smoothing using BufferListUtilities
            _rawKBuffer.Update(SmoothPeriods, rawK);

            switch (MovingAverageType)
            {
                case MaType.SMA:
                    if (_rawKBuffer.Count == SmoothPeriods)
                    {
                        smoothK = _rawKBuffer.Average();
                    }

                    break;

                case MaType.SMMA:
                    // Re/initialize with SMA when _prevSmoothK is NaN
                    // (happens at start or after NaN input values)
                    // This matches standard SMMA pattern (see Alligator, SMMA indicators)
                    if (double.IsNaN(_prevSmoothK))
                    {
                        if (_rawKBuffer.Count == SmoothPeriods)
                        {
                            _prevSmoothK = _rawKBuffer.Average();
                        }
                    }

                    if (!double.IsNaN(_prevSmoothK))
                    {
                        smoothK = ((_prevSmoothK * (SmoothPeriods - 1)) + rawK) / SmoothPeriods;
                        _prevSmoothK = smoothK;
                    }

                    break;

                default:
                    throw new InvalidOperationException("Invalid Stochastic moving average type.");
            }
        }

        // Calculate %D signal line - logic matches StaticSeries
        double signal = double.NaN;
        if (SignalPeriods <= 1)
        {
            signal = smoothK;
        }
        else if (Count >= SignalPeriods && !double.IsNaN(smoothK)) // This matches StaticSeries: i >= signalPeriods
        {
            // Update smooth K buffer for signal calculation using BufferListUtilities
            _smoothKBuffer.Update(SignalPeriods, smoothK);

            switch (MovingAverageType)
            {
                case MaType.SMA:
                    if (_smoothKBuffer.Count == SignalPeriods)
                    {
                        signal = _smoothKBuffer.Average();
                    }

                    break;

                case MaType.SMMA:
                    // Re/initialize with SMA when _prevSignal is NaN
                    // (happens at start or after NaN input values)
                    // This matches standard SMMA pattern (see Alligator, SMMA indicators)
                    if (double.IsNaN(_prevSignal))
                    {
                        if (_smoothKBuffer.Count == SignalPeriods)
                        {
                            _prevSignal = _smoothKBuffer.Average();
                        }
                    }

                    if (!double.IsNaN(_prevSignal))
                    {
                        signal = ((_prevSignal * (SignalPeriods - 1)) + smoothK) / SignalPeriods;
                        _prevSignal = signal;
                    }

                    break;

                default:
                    throw new InvalidOperationException("Invalid Stochastic moving average type.");
            }
        }

        // Calculate %J
        double percentJ = (KFactor * smoothK) - (DFactor * signal);

        // Add result to the list
        AddInternal(new StochResult(
            Timestamp: timestamp,
            Oscillator: smoothK.NaN2Null(),
            Signal: signal.NaN2Null(),
            PercentJ: percentJ.NaN2Null()));
    }

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
        _hlcBuffer.Clear();
        _rawKBuffer.Clear();
        _smoothKBuffer.Clear();
        _prevSmoothK = double.NaN;
        _prevSignal = double.NaN;
    }
}
