namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a Stochastic Momentum Index (SMI) stream hub that calculates SMI with signal line.
/// </summary>
public sealed class SmiHub
    : ChainHub<IQuote, SmiResult>, ISmi
{

    // Rolling windows for O(1) high/low tracking
    private readonly RollingWindowMax<double> _highWindow;
    private readonly RollingWindowMin<double> _lowWindow;

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

        // Initialize rolling windows for O(1) amortized max/min tracking
        _highWindow = new RollingWindowMax<double>(lookbackPeriods);
        _lowWindow = new RollingWindowMin<double>(lookbackPeriods);

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
        double hH = _highWindow.GetMax();
        double lL = _lowWindow.GetMin();

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
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset state variables
        _lastSmEma1 = double.NaN;
        _lastSmEma2 = double.NaN;
        _lastHlEma1 = double.NaN;
        _lastHlEma2 = double.NaN;
        _lastSignal = double.NaN;

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
}
