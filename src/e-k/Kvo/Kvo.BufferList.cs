namespace Skender.Stock.Indicators;

/// <summary>
/// Klinger Volume Oscillator (KVO) from incremental quote values.
/// </summary>
public class KvoList : BufferList<KvoResult>, IIncrementFromQuote, IKvo
{
    private readonly int _fastPeriods;
    private readonly int _slowPeriods;
    private readonly double _kFast;
    private readonly double _kSlow;
    private readonly double _kSignal;

    private double _prevHlc;
    private double _prevTrend;
    private double _prevDm;
    private double _prevCm;
    private double _prevVfFastEma;
    private double _prevVfSlowEma;
    private double _sumVf;

    /// <summary>
    /// Initializes a new instance of the <see cref="KvoList"/> class.
    /// </summary>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="signalPeriods"/> is invalid.</exception>
    public KvoList(
        int fastPeriods = 34,
        int slowPeriods = 55,
        int signalPeriods = 13)
    {
        Kvo.Validate(fastPeriods, slowPeriods, signalPeriods);
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;
        SignalPeriods = signalPeriods;

        _fastPeriods = fastPeriods;
        _slowPeriods = slowPeriods;
        _kFast = 2d / (fastPeriods + 1);
        _kSlow = 2d / (slowPeriods + 1);
        _kSignal = 2d / (signalPeriods + 1);

        _prevHlc = 0;
        _prevTrend = 0;
        _prevDm = 0;
        _prevCm = 0;
        _prevVfFastEma = 0;
        _prevVfSlowEma = 0;
        _sumVf = 0;

        Name = $"KVO({fastPeriods}, {slowPeriods}, {signalPeriods})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KvoList"/> class with initial quotes.
    /// </summary>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public KvoList(
        int fastPeriods,
        int slowPeriods,
        int signalPeriods,
        IReadOnlyList<IQuote> quotes)
        : this(fastPeriods, slowPeriods, signalPeriods) => Add(quotes);

    /// <inheritdoc />
    public int FastPeriods { get; init; }

    /// <inheritdoc />
    public int SlowPeriods { get; init; }

    /// <inheritdoc />
    public int SignalPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        double high = (double)quote.High;
        double low = (double)quote.Low;
        double close = (double)quote.Close;
        double volume = (double)quote.Volume;

        double? kvo = null;
        double? sig = null;

        // trend basis
        double hlc = high + low + close;

        // daily measurement
        double dm = high - low;

        if (Count <= 0)
        {
            _prevHlc = hlc;
            AddInternal(new KvoResult(quote.Timestamp));
            return;
        }

        // trend direction
        double trend = hlc > _prevHlc ? 1 : -1;

        if (Count <= 1)
        {
            _prevHlc = hlc;
            _prevTrend = trend;
            _prevDm = dm;
            _prevCm = 0;
            AddInternal(new KvoResult(quote.Timestamp));
            return;
        }

        // cumulative measurement
        double cm = trend == _prevTrend ?
                _prevCm + dm : _prevDm + dm;

        // volume force (VF)
        double vf = dm == cm || volume == 0 ? 0
            : dm == 0 ? volume * 2d * trend * 100d
            : cm != 0 ? volume * Math.Abs(2d * ((dm / cm) - 1)) * trend * 100d
            : 0;

        // accumulate VF for EMA initialization (starting from period 2)
        if (Count > 1)
        {
            _sumVf += vf;
        }

        // fast-period EMA of VF
        double vfFastEma = 0;
        if (Count > _fastPeriods + 1)
        {
            vfFastEma = (vf * _kFast) + (_prevVfFastEma * (1 - _kFast));
        }
        else if (Count == _fastPeriods + 1)
        {
            vfFastEma = _sumVf / _fastPeriods;
        }

        // slow-period EMA of VF
        double vfSlowEma = 0;
        if (Count > _slowPeriods + 1)
        {
            vfSlowEma = (vf * _kSlow) + (_prevVfSlowEma * (1 - _kSlow));
        }
        else if (Count == _slowPeriods + 1)
        {
            vfSlowEma = _sumVf / _slowPeriods;
        }

        // Klinger Oscillator
        if (Count >= _slowPeriods + 1)
        {
            kvo = vfFastEma - vfSlowEma;

            // Signal
            if (Count > _slowPeriods + SignalPeriods)
            {
                double prevSignal = this[^1].Signal ?? 0;
                sig = (kvo * _kSignal) + (prevSignal * (1 - _kSignal));
            }
            else if (Count == _slowPeriods + SignalPeriods)
            {
                // Sum of KVO values for signal initialization
                double sum = 0;
                for (int p = _slowPeriods + 1; p < Count; p++)
                {
                    sum += this[p].Oscillator ?? 0;
                }

                sum += kvo ?? 0;
                sig = sum / SignalPeriods;
            }
        }

        AddInternal(new KvoResult(
            Timestamp: quote.Timestamp,
            Oscillator: kvo,
            Signal: sig));

        // Save state for next iteration
        _prevHlc = hlc;
        _prevTrend = trend;
        _prevDm = dm;
        _prevCm = cm;
        _prevVfFastEma = vfFastEma;
        _prevVfSlowEma = vfSlowEma;
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

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
        _prevHlc = 0;
        _prevTrend = 0;
        _prevDm = 0;
        _prevCm = 0;
        _prevVfFastEma = 0;
        _prevVfSlowEma = 0;
        _sumVf = 0;
    }
}

public static partial class Kvo
{
    /// <summary>
    /// Creates a buffer list for Klinger Volume Oscillator (KVO) calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="fastPeriods">Number of periods for the fast moving average</param>
    /// <param name="slowPeriods">Number of periods for the slow moving average</param>
    /// <param name="signalPeriods">Number of periods for the signal line</param>
    public static KvoList ToKvoList(
        this IReadOnlyList<IQuote> quotes,
        int fastPeriods = 34,
        int slowPeriods = 55,
        int signalPeriods = 13)
        => new(fastPeriods, slowPeriods, signalPeriods) { quotes };
}
