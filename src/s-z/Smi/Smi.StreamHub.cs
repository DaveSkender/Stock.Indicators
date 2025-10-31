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
    private double lastSmEma1;
    private double lastSmEma2;
    private double lastHlEma1;
    private double lastHlEma2;
    private double lastSignal;

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

        ValidateFinite(high, nameof(IQuote.High), item.Timestamp);
        ValidateFinite(low, nameof(IQuote.Low), item.Timestamp);
        ValidateFinite(close, nameof(IQuote.Close), item.Timestamp);

        // Normal incremental update - O(1) amortized operation
        // Using monotonic deque pattern eliminates O(n) linear scans on every quote
        _highWindow.Add(high);
        _lowWindow.Add(low);

        double smi = double.NaN;
        double signal = double.NaN;

        if (i >= LookbackPeriods - 1)
        {
            // Use O(1) max/min retrieval from rolling windows
            double hH = _highWindow.Max;
            double lL = _lowWindow.Min;

            // Calculate distance from midpoint and range
            double sm = close - (0.5d * (hH + lL));
            double hl = hH - lL;

            // Initialize last EMA values on first calculation
            if (i == LookbackPeriods - 1)
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
            smi = 100 * (smEma2 / (0.5 * hlEma2));

            // Initialize signal line on first calculation
            if (i == LookbackPeriods - 1)
            {
                lastSignal = smi;
            }

            // Signal line
            signal = lastSignal + (kS * (smi - lastSignal));

            // Carryover values for next iteration
            lastSmEma1 = smEma1;
            lastSmEma2 = smEma2;
            lastHlEma1 = hlEma1;
            lastHlEma2 = hlEma2;
            lastSignal = signal;
        }

        SmiResult result = new(
            Timestamp: item.Timestamp,
            Smi: smi.NaN2Null(),
            Signal: signal.NaN2Null());

        return (result, i);
    }

    private static void ValidateFinite(double value, string paramName, DateTime timestamp)
    {
        if (!double.IsFinite(value))
        {
            string message = FormattableString.Invariant(
                $"Quote at {timestamp:O} contains a non-finite {paramName} value.");
            throw new InvalidQuotesException(paramName, value, message);
        }
    }

    /// <summary>
    /// Restores the SMI calculation state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset state variables
        lastSmEma1 = 0;
        lastSmEma2 = 0;
        lastHlEma1 = 0;
        lastHlEma2 = 0;
        lastSignal = 0;

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

            ValidateFinite(high, nameof(IQuote.High), q.Timestamp);
            ValidateFinite(low, nameof(IQuote.Low), q.Timestamp);
            ValidateFinite(close, nameof(IQuote.Close), q.Timestamp);

            // Add to rolling windows (maintains O(1) amortized operation)
            _highWindow.Add(high);
            _lowWindow.Add(low);

            // Calculate EMA state only after warmup
            if (p >= LookbackPeriods - 1)
            {
                // Use O(1) max/min retrieval from rolling windows
                double hH = _highWindow.Max;
                double lL = _lowWindow.Min;

                // Calculate distance from midpoint and range
                double sm = close - (0.5d * (hH + lL));
                double hl = hH - lL;

                // Initialize last EMA values on first calculation
                if (p == LookbackPeriods - 1)
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
                double smi = 100 * (smEma2 / (0.5 * hlEma2));

                // Initialize signal line on first calculation
                if (p == LookbackPeriods - 1)
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
