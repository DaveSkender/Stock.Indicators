namespace Skender.Stock.Indicators;

// STOCHASTIC RSI (STREAM HUB)


/// <summary>
/// Represents a Stochastic RSI stream hub that calculates Stochastic oscillator on RSI values.
/// </summary>
public sealed class StochRsiHub
    : ChainProvider<IReusable, StochRsiResult>
{
    private readonly string hubName;
    /// <summary>
    /// Internal RSI hub for incremental RSI calculation
    /// </summary>
    private readonly RsiHub rsiHub;

    /// <summary>
    /// Rolling windows for O(1) RSI max/min tracking
    /// </summary>
    private readonly RollingWindowMax<double> _rsiMaxWindow;
    private readonly RollingWindowMin<double> _rsiMinWindow;

    /// <summary>
    /// Rolling window for %K smoothing
    /// </summary>
    private readonly Queue<double> kBuffer;
    /// <summary>
    /// Rolling window for signal line calculation
    /// </summary>
    private readonly Queue<double> signalBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="StochRsiHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation.</param>
    /// <param name="stochPeriods">The number of periods for the Stochastic calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    internal StochRsiHub(
        IChainProvider<IReusable> provider,
        int rsiPeriods = 14,
        int stochPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 1) : base(provider)
    {
        StochRsi.Validate(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        RsiPeriods = rsiPeriods;
        StochPeriods = stochPeriods;
        SignalPeriods = signalPeriods;
        SmoothPeriods = smoothPeriods;

        hubName = $"STOCH-RSI({rsiPeriods},{stochPeriods},{signalPeriods},{smoothPeriods})";

        // Create internal RSI hub for incremental RSI calculation
        rsiHub = provider.ToRsiHub(rsiPeriods);

        // Rolling windows for O(1) RSI max/min tracking
        _rsiMaxWindow = new RollingWindowMax<double>(stochPeriods);
        _rsiMinWindow = new RollingWindowMin<double>(stochPeriods);

        // Buffers for rolling windows
        kBuffer = new Queue<double>(smoothPeriods);
        signalBuffer = new Queue<double>(signalPeriods);

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of periods for the RSI calculation.
    /// </summary>
    public int RsiPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the Stochastic calculation.
    /// </summary>
    public int StochPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the signal line.
    /// </summary>
    public int SignalPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for smoothing.
    /// </summary>
    public int SmoothPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (StochRsiResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? stochRsi = null;
        double? signal = null;

        // Get RSI value from the internal hub (leveraging the fixed RSI StreamHub from Phase 3)
        RsiResult? rsiResult = rsiHub.Results.ElementAtOrDefault(i);
        double? rsiValue = rsiResult?.Rsi;

        // Only process if we have a valid RSI value
        if (rsiValue.HasValue)
        {
            (double? oscillator, double? oscillatorSignal) = UpdateOscillatorState(rsiValue.Value);

            if (oscillator.HasValue)
            {
                stochRsi = oscillator;
                signal = oscillatorSignal;
            }
        }

        // candidate result
        StochRsiResult result = new(
            Timestamp: item.Timestamp,
            StochRsi: stochRsi,
            Signal: signal);

        return (result, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int providerIndex = ProviderCache.IndexGte(timestamp);
        if (providerIndex == -1)
        {
            providerIndex = ProviderCache.Count;
        }

        // Rebuild underlying RSI hub so replay uses fresh RSI values
        rsiHub.Rebuild(timestamp);

        // Reset state and replay historical RSI values up to the rebuild index
        _rsiMaxWindow.Clear();
        _rsiMinWindow.Clear();
        kBuffer.Clear();
        signalBuffer.Clear();

        if (providerIndex <= 0)
        {
            return;
        }

        IReadOnlyList<RsiResult> rsiResults = rsiHub.Results;
        int replayLimit = Math.Min(providerIndex, rsiResults.Count);

        for (int i = 0; i < replayLimit; i++)
        {
            RsiResult historical = rsiResults[i];
            if (historical.Rsi.HasValue)
            {
                _ = UpdateOscillatorState(historical.Rsi.Value);
            }
        }
    }

    private (double? stochRsi, double? signal) UpdateOscillatorState(double rsiValue)
    {
        // Add RSI value to rolling windows
        _rsiMaxWindow.Add(rsiValue);
        _rsiMinWindow.Add(rsiValue);

        if (_rsiMaxWindow.Count != StochPeriods)
        {
            return (null, null);
        }

        // Get high/low RSI from rolling windows (O(1))
        double highRsi = _rsiMaxWindow.Max;
        double lowRsi = _rsiMinWindow.Min;

        double k = lowRsi != highRsi
            ? 100d * (rsiValue - lowRsi) / (highRsi - lowRsi)
            : 0d;

        if (SmoothPeriods > 1)
        {
            kBuffer.Enqueue(k);
            if (kBuffer.Count > SmoothPeriods)
            {
                _ = kBuffer.Dequeue();
            }

            if (kBuffer.Count != SmoothPeriods)
            {
                return (null, null);
            }

            double sumK = 0d;
            foreach (double item in kBuffer)
            {
                sumK += item;
            }

            k = sumK / SmoothPeriods;
        }

        signalBuffer.Enqueue(k);
        if (signalBuffer.Count > SignalPeriods)
        {
            _ = signalBuffer.Dequeue();
        }

        double? signal = null;
        if (signalBuffer.Count == SignalPeriods)
        {
            double sumSignal = 0d;
            foreach (double item in signalBuffer)
            {
                sumSignal += item;
            }

            signal = sumSignal / SignalPeriods;
        }

        return (k, signal);
    }
}

public static partial class StochRsi
{
    /// <summary>
    /// Converts the chain provider to a Stochastic RSI hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation.</param>
    /// <param name="stochPeriods">The number of periods for the Stochastic calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    /// <returns>A Stochastic RSI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static StochRsiHub ToStochRsiHub(
        this IChainProvider<IReusable> chainProvider,
        int rsiPeriods = 14,
        int stochPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 1)
        => new(chainProvider, rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);


    /// <summary>
    /// Creates a StochRsi hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation.</param>
    /// <param name="stochPeriods">The number of periods for the Stochastic calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    /// <returns>An instance of <see cref="StochRsiHub"/>.</returns>
    public static StochRsiHub ToStochRsiHub(
        this IReadOnlyList<IQuote> quotes, int rsiPeriods = 14, int stochPeriods = 14, int signalPeriods = 3, int smoothPeriods = 1)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStochRsiHub(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);
    }
}
