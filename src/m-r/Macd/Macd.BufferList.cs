namespace Skender.Stock.Indicators;

/// <summary>
/// MACD (Moving Average Convergence Divergence) from incremental reusable values.
/// </summary>
public class MacdList : IndicatorBufferListBase<MacdResult>, IMultiPeriodIndicator
{
    private readonly Queue<double> _fastBuffer;
    private readonly Queue<double> _slowBuffer;
    private readonly Queue<double> _macdBuffer;
    
    private double _fastBufferSum;
    private double _slowBufferSum;
    private double _macdBufferSum;
    
    private double? _lastEmaFast;
    private double? _lastEmaSlow;
    private double? _lastEmaMacd;
    
    private readonly double kFast;
    private readonly double kSlow;
    private readonly double kMacd;

    /// <summary>
    /// Initializes a new instance of the <see cref="MacdList"/> class.
    /// </summary>
    /// <param name="fastPeriods">The number of periods for the fast EMA. Default is 12.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA. Default is 26.</param>
    /// <param name="signalPeriods">The number of periods for the signal line. Default is 9.</param>
    public MacdList(int fastPeriods = 12, int slowPeriods = 26, int signalPeriods = 9)
    {
        IndicatorUtilities.ValidateMultiPeriods(fastPeriods, slowPeriods, signalPeriods, "MACD");
        
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;
        SignalPeriods = signalPeriods;
        LookbackPeriods = slowPeriods; // Use slow periods as the primary lookback

        kFast = 2d / (fastPeriods + 1);
        kSlow = 2d / (slowPeriods + 1);
        kMacd = 2d / (signalPeriods + 1);

        _fastBuffer = new Queue<double>(fastPeriods);
        _slowBuffer = new Queue<double>(slowPeriods);
        _macdBuffer = new Queue<double>(signalPeriods);
        
        _fastBufferSum = 0;
        _slowBufferSum = 0;
        _macdBufferSum = 0;
        
        _lastEmaFast = null;
        _lastEmaSlow = null;
        _lastEmaMacd = null;
    }

    /// <inheritdoc/>
    public override int LookbackPeriods { get; }

    /// <inheritdoc/>
    public int FastPeriods { get; }

    /// <inheritdoc/>
    public int SlowPeriods { get; }

    /// <inheritdoc/>
    public int SignalPeriods { get; }

    /// <inheritdoc/>
    public override void Add(DateTime timestamp, double value)
    {
        // Update fast buffer
        if (_fastBuffer.Count == FastPeriods)
        {
            _fastBufferSum -= _fastBuffer.Dequeue();
        }
        _fastBuffer.Enqueue(value);
        _fastBufferSum += value;

        // Update slow buffer
        if (_slowBuffer.Count == SlowPeriods)
        {
            _slowBufferSum -= _slowBuffer.Dequeue();
        }
        _slowBuffer.Enqueue(value);
        _slowBufferSum += value;

        // Calculate Fast EMA
        double? emaFast = null;
        if (Count >= FastPeriods - 1)
        {
            if (_lastEmaFast == null)
            {
                emaFast = _fastBufferSum / FastPeriods; // Initialize with SMA
            }
            else
            {
                emaFast = Ema.Increment(kFast, _lastEmaFast.Value, value);
            }
            _lastEmaFast = emaFast;
        }

        // Calculate Slow EMA
        double? emaSlow = null;
        if (Count >= SlowPeriods - 1)
        {
            if (_lastEmaSlow == null)
            {
                emaSlow = _slowBufferSum / SlowPeriods; // Initialize with SMA
            }
            else
            {
                emaSlow = Ema.Increment(kSlow, _lastEmaSlow.Value, value);
            }
            _lastEmaSlow = emaSlow;
        }

        // Calculate MACD
        double? macd = null;
        if (emaFast.HasValue && emaSlow.HasValue)
        {
            macd = emaFast.Value - emaSlow.Value;
        }

        // Calculate Signal
        double? signal = null;
        if (macd.HasValue)
        {
            // Update MACD buffer for signal calculation
            if (_macdBuffer.Count == SignalPeriods)
            {
                _macdBufferSum -= _macdBuffer.Dequeue();
            }
            _macdBuffer.Enqueue(macd.Value);
            _macdBufferSum += macd.Value;

            if (Count >= SignalPeriods + SlowPeriods - 2)
            {
                if (_lastEmaMacd == null)
                {
                    signal = _macdBufferSum / SignalPeriods; // Initialize with SMA
                }
                else
                {
                    signal = Ema.Increment(kMacd, _lastEmaMacd.Value, macd.Value);
                }
                _lastEmaMacd = signal;
            }
        }

        // Calculate Histogram
        double? histogram = null;
        if (macd.HasValue && signal.HasValue)
        {
            histogram = macd.Value - signal.Value;
        }

        // Add result to list
        ((List<MacdResult>)this).Add(new MacdResult(
            timestamp,
            macd,
            signal,
            histogram,
            emaFast,
            emaSlow));
    }
}