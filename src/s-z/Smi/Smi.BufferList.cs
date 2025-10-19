namespace Skender.Stock.Indicators;

/// <summary>
/// Stochastic Momentum Index (SMI) from incremental quotes.
/// </summary>
public class SmiList : BufferList<SmiResult>, IIncrementFromQuote, ISmi
{
    private readonly Queue<(double High, double Low, double Close)> _lookbackBuffer;
    private double _lastSmEma1;
    private double _lastSmEma2;
    private double _lastHlEma1;
    private double _lastHlEma2;
    private double _lastSignal;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmiList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods for the lookback window.</param>
    /// <param name="firstSmoothPeriods">The number of periods for the first smoothing.</param>
    /// <param name="secondSmoothPeriods">The number of periods for the second smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line smoothing.</param>
    public SmiList(
        int lookbackPeriods = 13,
        int firstSmoothPeriods = 25,
        int secondSmoothPeriods = 2,
        int signalPeriods = 3)
    {
        Smi.Validate(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);
        LookbackPeriods = lookbackPeriods;
        FirstSmoothPeriods = firstSmoothPeriods;
        SecondSmoothPeriods = secondSmoothPeriods;
        SignalPeriods = signalPeriods;

        K1 = 2d / (firstSmoothPeriods + 1);
        K2 = 2d / (secondSmoothPeriods + 1);
        KS = 2d / (signalPeriods + 1);

        _lookbackBuffer = new Queue<(double, double, double)>(lookbackPeriods);
        _isInitialized = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmiList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods for the lookback window.</param>
    /// <param name="firstSmoothPeriods">The number of periods for the first smoothing.</param>
    /// <param name="secondSmoothPeriods">The number of periods for the second smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line smoothing.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public SmiList(
        int lookbackPeriods,
        int firstSmoothPeriods,
        int secondSmoothPeriods,
        int signalPeriods,
        IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods for the lookback window.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the first smoothing.
    /// </summary>
    public int FirstSmoothPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the second smoothing.
    /// </summary>
    public int SecondSmoothPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the signal line smoothing.
    /// </summary>
    public int SignalPeriods { get; init; }

    /// <summary>
    /// Gets the smoothing factor for the first EMA.
    /// </summary>
    public double K1 { get; private init; }

    /// <summary>
    /// Gets the smoothing factor for the second EMA.
    /// </summary>
    public double K2 { get; private init; }

    /// <summary>
    /// Gets the smoothing factor for the signal line.
    /// </summary>
    public double KS { get; private init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;
        double high = (double)quote.High;
        double low = (double)quote.Low;
        double close = (double)quote.Close;

        // Update lookback buffer
        _lookbackBuffer.Update(LookbackPeriods, (high, low, close));

        double? smi = null;
        double? signal = null;

        // Calculate when we have enough data
        if (_lookbackBuffer.Count == LookbackPeriods)
        {
            // Find highest high and lowest low in the lookback period
            double hH = double.MinValue;
            double lL = double.MaxValue;

            foreach ((double h, double l, double _) in _lookbackBuffer)
            {
                if (h > hH)
                {
                    hH = h;
                }

                if (l < lL)
                {
                    lL = l;
                }
            }

            double sm = close - (0.5d * (hH + lL));
            double hl = hH - lL;

            // Initialize last EMA values on first calculation
            if (!_isInitialized)
            {
                _lastSmEma1 = sm;
                _lastSmEma2 = _lastSmEma1;
                _lastHlEma1 = hl;
                _lastHlEma2 = _lastHlEma1;
                _isInitialized = true;
            }

            // First smoothing
            double smEma1 = _lastSmEma1 + (K1 * (sm - _lastSmEma1));
            double hlEma1 = _lastHlEma1 + (K1 * (hl - _lastHlEma1));

            // Second smoothing
            double smEma2 = _lastSmEma2 + (K2 * (smEma1 - _lastSmEma2));
            double hlEma2 = _lastHlEma2 + (K2 * (hlEma1 - _lastHlEma2));

            // Stochastic momentum index
            smi = 100 * (smEma2 / (0.5 * hlEma2));

            // Initialize signal line on first SMI calculation
            if (_lastSignal == 0 && Count == LookbackPeriods - 1)
            {
                _lastSignal = smi.Value;
            }

            // Signal line
            if (Count >= LookbackPeriods - 1)
            {
                signal = _lastSignal + (KS * (smi.Value - _lastSignal));
                _lastSignal = signal.Value;
            }

            // Carryover values
            _lastSmEma1 = smEma1;
            _lastSmEma2 = smEma2;
            _lastHlEma1 = hlEma1;
            _lastHlEma2 = hlEma2;
        }

        AddInternal(new SmiResult(timestamp, smi, signal));
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
        _lookbackBuffer.Clear();
        _lastSmEma1 = 0;
        _lastSmEma2 = 0;
        _lastHlEma1 = 0;
        _lastHlEma2 = 0;
        _lastSignal = 0;
        _isInitialized = false;
    }
}

public static partial class Smi
{
    /// <summary>
    /// Creates a buffer list for Stochastic Momentum Index (SMI) calculations.
    /// </summary>
    public static SmiList ToSmiList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 13,
        int firstSmoothPeriods = 25,
        int secondSmoothPeriods = 2,
        int signalPeriods = 3)
        => new(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods) { (IReadOnlyList<IQuote>)quotes };
}
