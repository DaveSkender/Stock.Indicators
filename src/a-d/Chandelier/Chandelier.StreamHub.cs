namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Chandelier Exit.
/// </summary>
public class ChandelierHub
    : ChainHub<AtrResult, ChandelierResult>, IChandelier
{
    private readonly AtrHub atrHub;
    private readonly IReadOnlyList<IQuote> quoteCache;
    private readonly RollingWindowMax<double> _highWindow;
    private readonly RollingWindowMin<double> _lowWindow;

    internal ChandelierHub(
        IChainProvider<AtrResult> provider,
        IQuoteProvider<IQuote> quoteProvider,
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

        // Store reference to ATR hub (which is now our provider)
        atrHub = (AtrHub)Provider;

        // Store reference to the underlying quote cache for High/Low access
        quoteCache = quoteProvider.Quotes;

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
        ToIndicator(AtrResult item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Add current quote to rolling windows using the index
        AddCurrentQuoteToWindows(i);

        // handle warmup periods
        if (i < LookbackPeriods)
        {
            return (new ChandelierResult(item.Timestamp, null), i);
        }

        // Get ATR value from the provider item (which is an AtrResult)
        // Since we're now subscribed to AtrHub, items are AtrResults
        double? atr = item.Atr;

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

    private void AddCurrentQuoteToWindows(int index)
    {
        // Access the quote from the underlying quote cache using the index
        IQuote quote = quoteCache[index];
        double high = (double)quote.High;
        double low = (double)quote.Low;

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

        // Rebuild windows from ProviderCache (AtrResults) up to the rollback point
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
            // Access the quote from the underlying quote cache
            IQuote quote = quoteCache[p];
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
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider.ToAtrHub(lookbackPeriods), quoteProvider, lookbackPeriods, multiplier, type);
    }
}
