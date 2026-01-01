namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Klinger Volume Oscillator (KVO) indicator.
/// </summary>
public static partial class Kvo
{
    /// <summary>
    /// Creates a KVO streaming hub from a quote provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <returns>A KVO hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    public static KvoHub ToKvoHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int fastPeriods = 34,
        int slowPeriods = 55,
        int signalPeriods = 13)
        => new(quoteProvider, fastPeriods, slowPeriods, signalPeriods);
}

/// <summary>
/// Streaming hub for Klinger Volume Oscillator (KVO) calculations.
/// </summary>
public class KvoHub
    : ChainHub<IQuote, KvoResult>, IKvo
{
    private readonly int _fastPeriods;
    private readonly int _slowPeriods;
    private readonly int _signalPeriods;
    private readonly double _kFast;
    private readonly double _kSlow;
    private readonly double _kSignal;

    /// <summary>
    /// State variables
    /// </summary>
    private double _prevHlc;
    private double _prevTrend;
    private double _prevDm;
    private double _prevCm;
    private double _prevVfFastEma;
    private double _prevVfSlowEma;
    private double _sumVf;

    internal KvoHub(
        IQuoteProvider<IQuote> provider,
        int fastPeriods,
        int slowPeriods,
        int signalPeriods) : base(provider)
    {
        Kvo.Validate(fastPeriods, slowPeriods, signalPeriods);

        _fastPeriods = fastPeriods;
        _slowPeriods = slowPeriods;
        _signalPeriods = signalPeriods;
        _kFast = 2d / (fastPeriods + 1);
        _kSlow = 2d / (slowPeriods + 1);
        _kSignal = 2d / (signalPeriods + 1);

        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;
        SignalPeriods = signalPeriods;

        Name = $"KVO({fastPeriods},{slowPeriods},{signalPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int FastPeriods { get; init; }

    /// <inheritdoc/>
    public int SlowPeriods { get; init; }

    /// <inheritdoc/>
    public int SignalPeriods { get; init; }
    /// <inheritdoc/>
    protected override (KvoResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double high = (double)item.High;
        double low = (double)item.Low;
        double close = (double)item.Close;
        double volume = (double)item.Volume;

        double? kvo = null;
        double? sig = null;

        // trend basis
        double hlc = high + low + close;

        // daily measurement
        double dm = high - low;

        if (i <= 0)
        {
            _prevHlc = hlc;
            KvoResult r0 = new(item.Timestamp);
            return (r0, i);
        }

        // trend direction
        double trend = hlc > _prevHlc ? 1 : -1;

        if (i <= 1)
        {
            _prevHlc = hlc;
            _prevTrend = trend;
            _prevDm = dm;
            _prevCm = 0;
            KvoResult r1 = new(item.Timestamp);
            return (r1, i);
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
        if (i > 1)
        {
            _sumVf += vf;
        }

        // fast-period EMA of VF
        double vfFastEma = 0;
        if (i > _fastPeriods + 1)
        {
            vfFastEma = (vf * _kFast) + (_prevVfFastEma * (1 - _kFast));
        }
        else if (i == _fastPeriods + 1)
        {
            vfFastEma = _sumVf / _fastPeriods;
        }

        // slow-period EMA of VF
        double vfSlowEma = 0;
        if (i > _slowPeriods + 1)
        {
            vfSlowEma = (vf * _kSlow) + (_prevVfSlowEma * (1 - _kSlow));
        }
        else if (i == _slowPeriods + 1)
        {
            vfSlowEma = _sumVf / _slowPeriods;
        }

        // Klinger Oscillator
        if (i >= _slowPeriods + 1)
        {
            kvo = vfFastEma - vfSlowEma;

            // Signal
            if (i > _slowPeriods + _signalPeriods)
            {
                double prevSignal = Cache[i - 1].Signal ?? 0;
                sig = (kvo * _kSignal) + (prevSignal * (1 - _kSignal));
            }
            else if (i == _slowPeriods + _signalPeriods)
            {
                // Sum of KVO values for signal initialization
                double sum = 0;
                for (int p = _slowPeriods + 1; p < i; p++)
                {
                    sum += Cache[p].Oscillator ?? 0;
                }

                sum += kvo ?? 0;
                sig = sum / _signalPeriods;
            }
        }

        KvoResult r = new(
            Timestamp: item.Timestamp,
            Oscillator: kvo,
            Signal: sig);

        // Save state for next iteration
        _prevHlc = hlc;
        _prevTrend = trend;
        _prevDm = dm;
        _prevCm = cm;
        _prevVfFastEma = vfFastEma;
        _prevVfSlowEma = vfSlowEma;

        return (r, i);
    }

    /// <summary>
    /// Restore rolling state up to the specified timestamp for accurate rebuilds.
    /// </summary>
    /// <param name="timestamp">Timestamp of record.</param>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset all state
        _prevHlc = 0;
        _prevTrend = 0;
        _prevDm = 0;
        _prevCm = 0;
        _prevVfFastEma = 0;
        _prevVfSlowEma = 0;
        _sumVf = 0;

        if (timestamp <= DateTime.MinValue || ProviderCache.Count == 0)
        {
            return;
        }

        // Find the first index at or after timestamp
        int index = ProviderCache.IndexGte(timestamp);

        if (index <= 0)
        {
            // Rolling back before all data, keep cleared state
            return;
        }

        // We need to rebuild state up to the index before timestamp
        int targetIndex = index - 1;

        // Rebuild state by recalculating all values up to targetIndex
        for (int i = 0; i <= targetIndex; i++)
        {
            IQuote item = ProviderCache[i];
            double high = (double)item.High;
            double low = (double)item.Low;
            double close = (double)item.Close;
            double volume = (double)item.Volume;

            // trend basis
            double hlc = high + low + close;

            // daily measurement
            double dm = high - low;

            if (i == 0)
            {
                _prevHlc = hlc;
                continue;
            }

            // trend direction
            double trend = hlc > _prevHlc ? 1 : -1;

            if (i == 1)
            {
                _prevHlc = hlc;
                _prevTrend = trend;
                _prevDm = dm;
                _prevCm = 0;
                continue;
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
            if (i > 1)
            {
                _sumVf += vf;
            }

            // fast-period EMA of VF
            double vfFastEma = 0;
            if (i > _fastPeriods + 1)
            {
                vfFastEma = (vf * _kFast) + (_prevVfFastEma * (1 - _kFast));
            }
            else if (i == _fastPeriods + 1)
            {
                vfFastEma = _sumVf / _fastPeriods;
            }

            // slow-period EMA of VF
            double vfSlowEma = 0;
            if (i > _slowPeriods + 1)
            {
                vfSlowEma = (vf * _kSlow) + (_prevVfSlowEma * (1 - _kSlow));
            }
            else if (i == _slowPeriods + 1)
            {
                vfSlowEma = _sumVf / _slowPeriods;
            }

            // Save state for next iteration
            _prevHlc = hlc;
            _prevTrend = trend;
            _prevDm = dm;
            _prevCm = cm;
            _prevVfFastEma = vfFastEma;
            _prevVfSlowEma = vfSlowEma;
        }
    }
}
