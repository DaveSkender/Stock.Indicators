using System.Collections;

namespace Skender.Stock.Indicators;

/// <summary>
/// Stochastic Oscillator from incremental quote values.
/// </summary>
public class StochList : List<StochResult>, IStoch, IBufferList
{
    private readonly Queue<double> _highBuffer;
    private readonly Queue<double> _lowBuffer;
    private readonly Queue<double> _closeBuffer;
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

        _highBuffer = new Queue<double>(lookbackPeriods);
        _lowBuffer = new Queue<double>(lookbackPeriods);
        _closeBuffer = new Queue<double>(lookbackPeriods);
        _rawKBuffer = new Queue<double>(smoothPeriods);
        _smoothKBuffer = new Queue<double>(signalPeriods);
    }

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
    public void Add(DateTime timestamp, double high, double low, double close)
    {
        // Update rolling buffers using BufferUtilities
        _highBuffer.Update(LookbackPeriods, high);
        _lowBuffer.Update(LookbackPeriods, low);
        _closeBuffer.Update(LookbackPeriods, close);

        // Calculate raw %K oscillator when we have enough data
        double? rawK = null;
        if (_highBuffer.Count == LookbackPeriods)
        {
            double highHigh = _highBuffer.Max();
            double lowLow = _lowBuffer.Min();

            if (highHigh - lowLow != 0)
            {
                rawK = 100.0 * (close - lowLow) / (highHigh - lowLow);
            }
            else
            {
                rawK = 0;
            }
        }

        // Calculate smoothed %K (final oscillator)
        double? smoothK = null;
        if (rawK.HasValue)
        {
            if (SmoothPeriods <= 1)
            {
                // No smoothing needed
                smoothK = rawK;
            }
            else
            {
                // Update raw K buffer for smoothing using BufferUtilities
                _rawKBuffer.Update(SmoothPeriods, rawK.Value);

                if (_rawKBuffer.Count == SmoothPeriods)
                {
                    smoothK = MovingAverageType switch {
                        MaType.SMA => _rawKBuffer.Average(),
                        MaType.SMMA => CalculateSmma(_rawKBuffer.Last(), ref _prevSmoothK, SmoothPeriods),
                        _ => throw new InvalidOperationException("Invalid Stochastic moving average type.")
                    };
                }
            }
        }

        // Calculate %D signal line
        double? signal = null;
        if (smoothK.HasValue)
        {
            if (SignalPeriods <= 1)
            {
                signal = smoothK;
            }
            else
            {
                // Update smooth K buffer for signal calculation using BufferUtilities
                _smoothKBuffer.Update(SignalPeriods, smoothK.Value);

                if (_smoothKBuffer.Count == SignalPeriods)
                {
                    signal = MovingAverageType switch {
                        MaType.SMA => _smoothKBuffer.Average(),
                        MaType.SMMA => CalculateSmma(_smoothKBuffer.Last(), ref _prevSignal, SignalPeriods),
                        _ => throw new InvalidOperationException("Invalid Stochastic moving average type.")
                    };
                }
            }
        }

        // Calculate %J
        double? percentJ = null;
        if (smoothK.HasValue && signal.HasValue)
        {
            percentJ = (KFactor * smoothK.Value) - (DFactor * signal.Value);
        }

        // Add result to the list
        base.Add(new StochResult(
            Timestamp: timestamp,
            Oscillator: smoothK,
            Signal: signal,
            PercentJ: percentJ));
    }

    /// <inheritdoc />
    public new void Clear()
    {
        base.Clear();
        _highBuffer.Clear();
        _lowBuffer.Clear();
        _closeBuffer.Clear();
        _rawKBuffer.Clear();
        _smoothKBuffer.Clear();
        _prevSmoothK = double.NaN;
        _prevSignal = double.NaN;
    }

    /// <summary>
    /// Calculates SMMA (Smoothed Moving Average) incrementally.
    /// </summary>
    /// <param name="currentValue">The current value to include.</param>
    /// <param name="previousSmma">Reference to the previous SMMA value.</param>
    /// <param name="periods">The number of periods for smoothing.</param>
    /// <returns>The calculated SMMA value.</returns>
    private static double CalculateSmma(double currentValue, ref double previousSmma, int periods)
    {
        if (double.IsNaN(previousSmma))
        {
            previousSmma = currentValue;
        }

        double smma = ((previousSmma * (periods - 1)) + currentValue) / periods;
        previousSmma = smma;
        return smma;
    }
}
