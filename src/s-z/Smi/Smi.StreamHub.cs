namespace Skender.Stock.Indicators;

// STOCHASTIC MOMENTUM INDEX (STREAM HUB)

/// <summary>
/// Represents a Stochastic Momentum Index (SMI) stream hub that calculates SMI with signal line.
/// </summary>
public sealed class SmiHub
    : ChainProvider<IQuote, SmiResult>, ISmi
{
    private readonly string hubName;

    // Rolling windows for O(1) high/low tracking
    private readonly RollingWindowMax<double> _highWindow;
    private readonly RollingWindowMin<double> _lowWindow;

    // State for EMA smoothing
    private double _lastSmEma1;
    private double _lastSmEma2;
    private double _lastHlEma1;
    private double _lastHlEma2;
    private double _lastSignal;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmiHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback window.</param>
    /// <param name="firstSmoothPeriods">The number of periods for the first smoothing.</param>
    /// <param name="secondSmoothPeriods">The number of periods for the second smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line smoothing.</param>
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

        hubName = $"SMI({lookbackPeriods},{firstSmoothPeriods},{secondSmoothPeriods},{signalPeriods})";

        // Initialize rolling windows for O(1) high/low tracking
        _highWindow = new RollingWindowMax<double>(lookbackPeriods);
        _lowWindow = new RollingWindowMin<double>(lookbackPeriods);

        _isInitialized = false;

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

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (SmiResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double high = (double)item.High;
        double low = (double)item.Low;
        double close = (double)item.Close;

        // Add to rolling windows
        _highWindow.Add(high);
        _lowWindow.Add(low);

        double? smi = null;
        double? signal = null;

        // Calculate when we have enough data
        if (i >= LookbackPeriods - 1)
        {
            // Get highest high and lowest low from rolling windows (O(1))
            double hH = _highWindow.Max;
            double lL = _lowWindow.Min;

            double sm = close - (0.5d * (hH + lL));
            double hl = hH - lL;

            // Initialize last EMA values on first calculation (when we have exactly LookbackPeriods of data)
            // Cache.Count is the count BEFORE adding current result, so check for LookbackPeriods - 1
            if (Cache.Count == LookbackPeriods - 1)
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
            if (Cache.Count == LookbackPeriods - 1)
            {
                _lastSignal = smi.Value;
            }

            // Signal line
            signal = _lastSignal + (KS * (smi.Value - _lastSignal));
            _lastSignal = signal.Value;

            // Carryover values for next iteration
            _lastSmEma1 = smEma1;
            _lastSmEma2 = smEma2;
            _lastHlEma1 = hlEma1;
            _lastHlEma2 = hlEma2;
        }

        // Candidate result
        SmiResult result = new(
            Timestamp: item.Timestamp,
            Smi: smi,
            Signal: signal);

        return (result, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear state
        _highWindow.Clear();
        _lowWindow.Clear();
        _lastSmEma1 = 0;
        _lastSmEma2 = 0;
        _lastHlEma1 = 0;
        _lastHlEma2 = 0;
        _lastSignal = 0;
        _isInitialized = false;

        // Rebuild from ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        int targetIndex = index - 1;
        int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);

        // Replay historical quotes to rebuild state
        int replayCount = 0;
        for (int p = startIdx; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];
            double high = (double)quote.High;
            double low = (double)quote.Low;
            double close = (double)quote.Close;

            // Add to rolling windows
            _highWindow.Add(high);
            _lowWindow.Add(low);

            // Recalculate state if we have enough data
            if (_highWindow.Count == LookbackPeriods)
            {
                double hH = _highWindow.Max;
                double lL = _lowWindow.Min;

                double sm = close - (0.5d * (hH + lL));
                double hl = hH - lL;

                // Initialize on first complete window
                if (replayCount == LookbackPeriods - 1)
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
                double smi = 100 * (smEma2 / (0.5 * hlEma2));

                // Initialize signal line on first SMI calculation
                if (replayCount == LookbackPeriods - 1)
                {
                    _lastSignal = smi;
                }

                // Signal line
                double signal = _lastSignal + (KS * (smi - _lastSignal));

                // Carryover values
                _lastSmEma1 = smEma1;
                _lastSmEma2 = smEma2;
                _lastHlEma1 = hlEma1;
                _lastHlEma2 = hlEma2;
                _lastSignal = signal;
            }
            replayCount++;
        }
    }
}

public static partial class Smi
{
    /// <summary>
    /// Creates a Stochastic Momentum Index (SMI) streaming hub from a quotes provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback window.</param>
    /// <param name="firstSmoothPeriods">The number of periods for the first smoothing.</param>
    /// <param name="secondSmoothPeriods">The number of periods for the second smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line smoothing.</param>
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

    /// <summary>
    /// Creates a Stochastic Momentum Index (SMI) hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback window.</param>
    /// <param name="firstSmoothPeriods">The number of periods for the first smoothing.</param>
    /// <param name="secondSmoothPeriods">The number of periods for the second smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line smoothing.</param>
    /// <returns>An instance of <see cref="SmiHub"/>.</returns>
    public static SmiHub ToSmiHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 13,
        int firstSmoothPeriods = 25,
        int secondSmoothPeriods = 2,
        int signalPeriods = 3)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSmiHub(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);
    }
}
