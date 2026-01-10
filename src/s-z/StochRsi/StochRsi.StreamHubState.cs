namespace Skender.Stock.Indicators;

/// <summary>
/// State object for Stochastic RSI streaming hub.
/// </summary>
/// <param name="RsiMaxCount">Count of values in the RSI max rolling window.</param>
/// <param name="RsiMinCount">Count of values in the RSI min rolling window.</param>
/// <param name="KBufferCount">Count of values in the K smoothing buffer.</param>
/// <param name="SignalBufferCount">Count of values in the signal buffer.</param>
public record StochRsiState(
    int RsiMaxCount,
    int RsiMinCount,
    int KBufferCount,
    int SignalBufferCount) : IHubState;

/// <summary>
/// Streaming hub for Stochastic RSI using state management.
/// </summary>
/// <remarks>
/// This implementation caches all internal state (RSI hub state, rolling windows, buffers)
/// for O(1) restoration during rapid same-candle updates.
/// </remarks>
public sealed class StochRsiHubState
    : ChainHubState<IReusable, StochRsiState, StochRsiResult>
{
    /// <summary>
    /// Internal RSI hub for incremental RSI calculation
    /// </summary>
    private readonly RsiHubState rsiHub;

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

    internal StochRsiHubState(
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

        Name = $"STOCH-RSI-State({rsiPeriods},{stochPeriods},{signalPeriods},{smoothPeriods})";

        // Create internal RSI hub for incremental RSI calculation
        rsiHub = provider.ToRsiHubState(rsiPeriods);

        // Rolling windows for O(1) RSI max/min tracking
        _rsiMaxWindow = new RollingWindowMax<double>(stochPeriods);
        _rsiMinWindow = new RollingWindowMin<double>(stochPeriods);

        // Buffers for rolling windows
        kBuffer = new Queue<double>(smoothPeriods);
        signalBuffer = new Queue<double>(signalPeriods);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int RsiPeriods { get; init; }

    /// <inheritdoc/>
    public int StochPeriods { get; init; }

    /// <inheritdoc/>
    public int SignalPeriods { get; init; }

    /// <inheritdoc/>
    public int SmoothPeriods { get; init; }

    /// <inheritdoc/>
    protected override (StochRsiResult result, StochRsiState state, int index)
        ToIndicatorState(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? stochRsi = null;
        double? signal = null;

        // Get RSI value from the internal hub
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

        // Capture current state (just counts, not full arrays)
        StochRsiState currentState = new(
            RsiMaxCount: _rsiMaxWindow.Count,
            RsiMinCount: _rsiMinWindow.Count,
            KBufferCount: kBuffer.Count,
            SignalBufferCount: signalBuffer.Count);

        // Candidate result
        StochRsiResult result = new(
            Timestamp: item.Timestamp,
            StochRsi: stochRsi,
            Signal: signal);

        return (result, currentState, i);
    }

    /// <inheritdoc/>
    protected override void RestorePreviousState(StochRsiState? previousState)
    {
        // For StochRSI, we always rebuild from RSI results since state is complex
        // This method is called during rapid same-candle updates, but we use RollbackState
        // The state object just tracks that state exists, not the full buffers
        if (previousState is null)
        {
            // Reset to initial state
            _rsiMaxWindow.Clear();
            _rsiMinWindow.Clear();
            kBuffer.Clear();
            signalBuffer.Clear();
        }
        // No explicit restoration - will be handled by RollbackState
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
        double highRsi = _rsiMaxWindow.GetMax();
        double lowRsi = _rsiMinWindow.GetMin();

        // Boundary detection to avoid floating-point precision errors at 0 and 100
        double k;
        if (lowRsi == highRsi)
        {
            k = 0d;
        }
        else if (rsiValue >= highRsi)
        {
            // Exact 100 when RSI equals or exceeds highRsi
            k = 100d;
        }
        else if (rsiValue <= lowRsi)
        {
            // Exact 0 when RSI equals or falls below lowRsi
            k = 0d;
        }
        else
        {
            k = 100d * (rsiValue - lowRsi) / (highRsi - lowRsi);
        }

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
    /// Converts the chain provider to a Stochastic RSI hub with state management.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation.</param>
    /// <param name="stochPeriods">The number of periods for the Stochastic calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    /// <returns>A Stochastic RSI hub with state management.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static StochRsiHubState ToStochRsiHubState(
        this IChainProvider<IReusable> chainProvider,
        int rsiPeriods = 14,
        int stochPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 1)
        => new(chainProvider, rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);
}
