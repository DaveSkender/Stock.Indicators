namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a Stochastic Momentum Index (SMI) stream hub that calculates SMI with signal line.
/// </summary>
public sealed class SmiHub
    : ChainHub<IQuote, SmiResult>, ISmi
{

    // Circular buffers for high/low tracking — avoids heap allocations per tick
    private readonly double[] _highs;
    private readonly double[] _lows;
    private int _windowHead;  // next write position in circular buffer
    private int _windowFill;  // items in buffer (0..LookbackPeriods)

    // State for EMA smoothing
    private double _lastSmEma1 = double.NaN;
    private double _lastSmEma2 = double.NaN;
    private double _lastHlEma1 = double.NaN;
    private double _lastHlEma2 = double.NaN;
    private double _lastSignal = double.NaN;

    internal SmiHub(
        IStreamObservable<IQuote> provider,
        int lookbackPeriods = 13,
        int firstSmoothPeriods = 25,
        int secondSmoothPeriods = 2,
        int signalPeriods = 3) : base(provider)
    {
        Smi.Validate(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);

        LookbackPeriods = lookbackPeriods;
        FirstSmoothPeriods = firstSmoothPeriods;
        SecondSmoothPeriods = secondSmoothPeriods;
        SignalPeriods = signalPeriods;

        K1 = 2d / (firstSmoothPeriods + 1);
        K2 = 2d / (secondSmoothPeriods + 1);
        KS = 2d / (signalPeriods + 1);

        Name = $"SMI({lookbackPeriods},{firstSmoothPeriods},{secondSmoothPeriods},{signalPeriods})";

        // Validate cache size for warmup requirements
        ValidateCacheSize(lookbackPeriods, Name);

        // Allocate circular buffers — fixed size, no per-tick heap allocations
        _highs = new double[lookbackPeriods];
        _lows = new double[lookbackPeriods];

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public int FirstSmoothPeriods { get; init; }

    /// <inheritdoc/>
    public int SecondSmoothPeriods { get; init; }

    /// <inheritdoc/>
    public int SignalPeriods { get; init; }

    /// <inheritdoc/>
    public double K1 { get; private init; }

    /// <inheritdoc/>
    public double K2 { get; private init; }

    /// <inheritdoc/>
    public double KS { get; private init; }
    /// <inheritdoc/>
    protected override (SmiResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double high = (double)item.High;
        double low = (double)item.Low;
        double close = (double)item.Close;

        // Write into circular buffer — no heap allocation, O(1) per tick
        AddToWindow(high, low);

        double smi = double.NaN;
        double signal = double.NaN;

        if (_windowFill == LookbackPeriods)
        {
            (smi, signal) = CalculateSmi(close);
        }

        SmiResult result = new(
            Timestamp: item.Timestamp,
            Smi: smi.NaN2Null(),
            Signal: signal.NaN2Null());

        return (result, i);
    }

    // Writes high/low into the circular buffer and advances the head pointer.
    private void AddToWindow(double high, double low)
    {
        _highs[_windowHead] = high;
        _lows[_windowHead] = low;
        _windowHead++;
        if (_windowHead >= LookbackPeriods)
        {
            _windowHead = 0;
        }

        if (_windowFill < LookbackPeriods)
        {
            _windowFill++;
        }
    }

    private (double smi, double signal) CalculateSmi(double close)
    {
        // Scan circular buffer for max high and min low — cache-friendly O(LookbackPeriods) scan
        // Eliminates heap allocations from monotonic-deque pattern; fast for small fixed windows
        double hH = _highs[0];
        double lL = _lows[0];
        for (int j = 1; j < LookbackPeriods; j++)
        {
            double h = _highs[j];
            double l = _lows[j];
            if (h > hH)
            {
                hH = h;
            }

            if (l < lL)
            {
                lL = l;
            }
        }

        // Calculate distance from midpoint and range
        double sm = close - (0.5d * (hH + lL));
        double hl = hH - lL;

        // Initialize last EMA values when no prior state exists
        if (double.IsNaN(_lastSmEma1))
        {
            _lastSmEma1 = sm;
            _lastSmEma2 = _lastSmEma1;
            _lastHlEma1 = hl;
            _lastHlEma2 = _lastHlEma1;
        }

        // First smoothing
        double smEma1 = _lastSmEma1 + (K1 * (sm - _lastSmEma1));
        double hlEma1 = _lastHlEma1 + (K1 * (hl - _lastHlEma1));

        // Second smoothing
        double smEma2 = _lastSmEma2 + (K2 * (smEma1 - _lastSmEma2));
        double hlEma2 = _lastHlEma2 + (K2 * (hlEma1 - _lastHlEma2));

        // Stochastic momentum index
        double smi = hlEma2 != 0 ? 100 * (smEma2 / (0.5 * hlEma2)) : double.NaN;

        // Initialize signal line when no prior state exists
        if (double.IsNaN(_lastSignal))
        {
            _lastSignal = smi;
        }

        // Signal line
        double signal = _lastSignal + (KS * (smi - _lastSignal));

        // Carryover values for next iteration
        _lastSmEma1 = smEma1;
        _lastSmEma2 = smEma2;
        _lastHlEma1 = hlEma1;
        _lastHlEma2 = hlEma2;
        _lastSignal = signal;

        return (smi, signal);
    }

    /// <summary>
    /// Restores the SMI calculation state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(int restoreIndex)
    {
        // Reset state variables
        _lastSmEma1 = double.NaN;
        _lastSmEma2 = double.NaN;
        _lastHlEma1 = double.NaN;
        _lastHlEma2 = double.NaN;
        _lastSignal = double.NaN;

        // Reset circular buffer
        _windowHead = 0;
        _windowFill = 0;

        if (restoreIndex < LookbackPeriods - 1)
        {
            return;
        }

        // Rebuild state from cache up to the rollback point
        for (int p = 0; p <= restoreIndex; p++)
        {
            IQuote q = ProviderCache[p];
            double high = (double)q.High;
            double low = (double)q.Low;
            double close = (double)q.Close;

            // Write into circular buffer
            AddToWindow(high, low);

            // Calculate EMA state only after warmup
            if (p >= LookbackPeriods - 1)
            {
                CalculateSmi(close);
            }
        }
    }

}

public static partial class Smi
{
    /// <summary>
    /// Creates a Stochastic Momentum Index (SMI) streaming hub from a quotes provider.
    /// </summary>
    /// <param name="quoteProvider">Quote provider.</param>
    /// <param name="lookbackPeriods">Number of periods for the lookback window.</param>
    /// <param name="firstSmoothPeriods">Number of periods for the first smoothing.</param>
    /// <param name="secondSmoothPeriods">Number of periods for the second smoothing.</param>
    /// <param name="signalPeriods">Number of periods for the signal line smoothing.</param>
    /// <returns>A Stochastic Momentum Index hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static SmiHub ToSmiHub(
        this IStreamObservable<IQuote> quoteProvider,
        int lookbackPeriods = 13,
        int firstSmoothPeriods = 25,
        int secondSmoothPeriods = 2,
        int signalPeriods = 3)
        => new(quoteProvider, lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);
}
