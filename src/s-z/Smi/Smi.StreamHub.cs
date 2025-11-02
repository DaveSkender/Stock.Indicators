namespace Skender.Stock.Indicators;

// STOCHASTIC MOMENTUM INDEX (STREAM HUB)

/// <summary>
/// Provides methods for creating Stochastic Momentum Index (SMI) hubs.
/// </summary>
public class SmiHub
    : StreamHub<IQuote, SmiResult>, ISmi
{
    #region fields

    private readonly string hubName;
    private readonly double k1; // First smoothing factor
    private readonly double k2; // Second smoothing factor
    private readonly double kS; // Signal smoothing factor
    private readonly RollingWindowMax<double> _highWindow;
    private readonly RollingWindowMin<double> _lowWindow;

    // State variables for incremental calculation
    private double lastSmEma1 = double.NaN;
    private double lastSmEma2 = double.NaN;
    private double lastHlEma1 = double.NaN;
    private double lastHlEma2 = double.NaN;
    private double lastSignal = double.NaN;

    #endregion fields

    #region constructors

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
        int lookbackPeriods,
        int firstSmoothPeriods,
        int secondSmoothPeriods,
        int signalPeriods) : base(provider)
    {
        Smi.Validate(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        LookbackPeriods = lookbackPeriods;
        FirstSmoothPeriods = firstSmoothPeriods;
        SecondSmoothPeriods = secondSmoothPeriods;
        SignalPeriods = signalPeriods;

        // Calculate EMA smoothing factors
        k1 = 2d / (firstSmoothPeriods + 1);
        k2 = 2d / (secondSmoothPeriods + 1);
        kS = 2d / (signalPeriods + 1);

        hubName = $"SMI({lookbackPeriods},{firstSmoothPeriods},{secondSmoothPeriods},{signalPeriods})";

        // Initialize rolling windows for O(1) amortized max/min tracking
        _highWindow = new RollingWindowMax<double>(lookbackPeriods);
        _lowWindow = new RollingWindowMin<double>(lookbackPeriods);

        Reinitialize();
    }

    #endregion constructors

    #region properties

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public int FirstSmoothPeriods { get; init; }

    /// <inheritdoc />
    public int SecondSmoothPeriods { get; init; }

    /// <inheritdoc />
    public int SignalPeriods { get; init; }

    #endregion properties

    #region methods

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

        // Normal incremental update - O(1) amortized operation
        // Using monotonic deque pattern eliminates O(n) linear scans on every quote
        _highWindow.Add(high);
        _lowWindow.Add(low);

        double smi = double.NaN;
        double signal = double.NaN;

        if (i >= LookbackPeriods - 1)
        {
            (smi, signal) = CalculateSmi(close);
        }

        SmiResult result = new(
            Timestamp: item.Timestamp,
            Smi: smi.NaN2Null(),
            Signal: signal.NaN2Null());

        return (result, i);
    }

    private (double smi, double signal) CalculateSmi(double close)
    {
        // Use O(1) max/min retrieval from rolling windows
        double hH = _highWindow.Max;
        double lL = _lowWindow.Min;

        // Calculate distance from midpoint and range
        double sm = close - (0.5d * (hH + lL));
        double hl = hH - lL;

        // Initialize last EMA values when no prior state exists
        if (double.IsNaN(lastSmEma1))
        {
            lastSmEma1 = sm;
            lastSmEma2 = lastSmEma1;
            lastHlEma1 = hl;
            lastHlEma2 = lastHlEma1;
        }

        // First smoothing
        double smEma1 = lastSmEma1 + (k1 * (sm - lastSmEma1));
        double hlEma1 = lastHlEma1 + (k1 * (hl - lastHlEma1));

        // Second smoothing
        double smEma2 = lastSmEma2 + (k2 * (smEma1 - lastSmEma2));
        double hlEma2 = lastHlEma2 + (k2 * (hlEma1 - lastHlEma2));

        // Stochastic momentum index
        double smi = hlEma2 != 0
            ? 100 * (smEma2 / (0.5 * hlEma2))
            : double.NaN;

        // Initialize signal line when no prior state exists
        if (double.IsNaN(lastSignal))
        {
            lastSignal = smi;
        }

        // Signal line
        double signal = lastSignal + (kS * (smi - lastSignal));

        // Carryover values for next iteration
        lastSmEma1 = smEma1;
        lastSmEma2 = smEma2;
        lastHlEma1 = hlEma1;
        lastHlEma2 = hlEma2;
        lastSignal = signal;

        return (smi, signal);
    }

    /// <summary>
    /// Restores the SMI calculation state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset state variables
        lastSmEma1 = double.NaN;
        lastSmEma2 = double.NaN;
        lastHlEma1 = double.NaN;
        lastHlEma2 = double.NaN;
        lastSignal = double.NaN;

        // Clear rolling windows
        _highWindow.Clear();
        _lowWindow.Clear();

        // Find the index up to which we need to rebuild
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0 || index < LookbackPeriods)
        {
            return;
        }

        // Rebuild state from cache up to the rollback point
        int targetIndex = index - 1;

        // Process each period to rebuild both windows and EMA state
        for (int p = 0; p <= targetIndex; p++)
        {
            IQuote q = ProviderCache[p];
            double high = (double)q.High;
            double low = (double)q.Low;
            double close = (double)q.Close;

            // Add to rolling windows (maintains O(1) amortized operation)
            _highWindow.Add(high);
            _lowWindow.Add(low);

            // Calculate EMA state only after warmup
            if (p >= LookbackPeriods - 1)
            {
                CalculateSmi(close);
            }
        }
    }

    #endregion methods
}


public static partial class Smi
{
    /// <summary>
    /// Converts the quote provider to a Stochastic Momentum Index hub.
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
    /// Creates a Smi hub from a collection of quotes.
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
