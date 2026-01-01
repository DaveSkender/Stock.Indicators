namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Chandelier Exit.
/// </summary>
public class ChandelierHub
    : StreamHub<IQuote, ChandelierResult>, IChandelier
{
    private readonly AtrHub atrHub;
    private readonly RollingWindowMax<double> _highWindow;
    private readonly RollingWindowMin<double> _lowWindow;

    internal ChandelierHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods,
        double multiplier,
        Direction type) : base(provider)
    {
        Chandelier.Validate(lookbackPeriods, multiplier);

        LookbackPeriods = lookbackPeriods;
        Multiplier = multiplier;
        Type = type;

        string typeName = type.ToString().ToUpperInvariant();
        Name = FormattableString.Invariant(
            $"CHEXIT({lookbackPeriods},{multiplier},{typeName})");

        // Initialize internal ATR hub to maintain streaming state
        atrHub = provider.ToAtrHub(lookbackPeriods);

        // Initialize rolling windows for O(1) amortized max/min tracking
        _highWindow = new RollingWindowMax<double>(lookbackPeriods);
        _lowWindow = new RollingWindowMin<double>(lookbackPeriods);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double Multiplier { get; init; }

    /// <inheritdoc/>
    public Direction Type { get; init; }

    /// <inheritdoc/>
    protected override (ChandelierResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Add current quote to rolling windows
        AddCurrentQuoteToWindows(item);

        // handle warmup periods
        if (i < LookbackPeriods)
        {
            return (new ChandelierResult(item.Timestamp, null), i);
        }

        // use cached ATR result from internal hub (O(1) lookup)
        // System invariant: atrHub.Results[i] must exist because atrHub subscribes
        // to the same provider and processes updates synchronously before this hub.
        // This bounds check defends against edge cases during initialization/rebuild.
        if (i >= atrHub.Results.Count)
        {
            throw new InvalidOperationException(
                $"ATR hub synchronization error: expected ATR result at index {i}, "
                + $"but atrHub.Results.Count is {atrHub.Results.Count}. "
                + "This indicates a state synchronization issue between chained hubs.");
        }

        double? atr = atrHub.Results[i].Atr;

        if (atr is null)
        {
            return (new ChandelierResult(item.Timestamp, null), i);
        }

        // Calculate exit using O(1) max/min retrieval from rolling windows
        double? exit = Type switch {
            Direction.Long => _highWindow.GetMax() - (atr.Value * Multiplier),
            Direction.Short => _lowWindow.GetMin() + (atr.Value * Multiplier),
            _ => throw new InvalidOperationException($"Unknown direction type: {Type}")
        };

        ChandelierResult r = new(
            Timestamp: item.Timestamp,
            ChandelierExit: exit);

        return (r, i);
    }

    private void AddCurrentQuoteToWindows(IQuote item)
    {
        double high = (double)item.High;
        double low = (double)item.Low;

        // Normal incremental update - O(1) amortized operation
        // Using monotonic deque pattern eliminates O(n) linear scans on every quote
        // NaN values are allowed and will propagate naturally through calculations
        _highWindow.Add(high);
        _lowWindow.Add(low);
    }

    /// <summary>
    /// Restores the rolling window state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear rolling windows
        _highWindow.Clear();
        _lowWindow.Clear();

        // Rebuild windows from ProviderCache up to the rollback point
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        // Rebuild up to the index before the rollback timestamp
        int targetIndex = index - 1;
        int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);

        for (int p = startIdx; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];
            double cachedHigh = (double)quote.High;
            double cachedLow = (double)quote.Low;

            _highWindow.Add(cachedHigh);
            _lowWindow.Add(cachedLow);
        }
    }
}

/// <summary>
/// Streaming hub for Chandelier Exit using a stream hub.
/// </summary>
public static partial class Chandelier
{
    /// <summary>
    /// Creates a Chandelier Exit streaming hub from a quotes provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier to apply to the ATR.</param>
    /// <param name="type">The type of Chandelier Exit to calculate (Long or Short).</param>
    /// <returns>An instance of <see cref="ChandelierHub"/>.</returns>
    public static ChandelierHub ToChandelierHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 22,
        double multiplier = 3,
        Direction type = Direction.Long)
             => new(quoteProvider, lookbackPeriods, multiplier, type);
}
