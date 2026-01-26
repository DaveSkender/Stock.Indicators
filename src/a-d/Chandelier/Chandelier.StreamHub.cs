namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Chandelier Exit.
/// </summary>
public class ChandelierHub
    : ChainHub<AtrResult, ChandelierResult>, IChandelier
{
    private readonly IQuoteProvider<IQuote> _quoteProvider;
    private readonly RollingWindowMax<double> _highWindow;
    private readonly RollingWindowMin<double> _lowWindow;

    internal ChandelierHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods,
        double multiplier,
        Direction type)
        : this(
            provider.ToAtrHub(lookbackPeriods),
            provider,
            lookbackPeriods,
            multiplier,
            type)
    { }

    internal ChandelierHub(
        AtrHub atrHub,
        IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods,
        double multiplier,
        Direction type)
        : base(atrHub)
    {
        ArgumentNullException.ThrowIfNull(atrHub);
        ArgumentNullException.ThrowIfNull(quoteProvider);
        Chandelier.Validate(lookbackPeriods, multiplier);

        _quoteProvider = quoteProvider;
        LookbackPeriods = lookbackPeriods;
        Multiplier = multiplier;
        Type = type;

        string typeName = type.ToString().ToUpperInvariant();
        Name = FormattableString.Invariant(
            $"CHEXIT({lookbackPeriods},{multiplier},{typeName})");

        // Initialize rolling windows for O(1) amortized max/min tracking
        _highWindow = new RollingWindowMax<double>(lookbackPeriods);
        _lowWindow = new RollingWindowMin<double>(lookbackPeriods);

        // Validate cache size for warmup requirements
        ValidateCacheSize(lookbackPeriods, Name);

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

        // Get the quote from the underlying quote provider's cache
        // to access High/Low values for rolling windows.
        // System invariant: _quoteProvider.Results[i] must exist because the ATR hub
        // is chained to this quote provider and processes quotes synchronously.
        // The index i from ProviderCache (ATR results) maps 1:1 to quote provider indices.
        IQuote quote = _quoteProvider.Results[i];

        // Add current quote to rolling windows
        AddCurrentQuoteToWindows(quote);

        // handle warmup periods
        if (i < LookbackPeriods)
        {
            return (new ChandelierResult(item.Timestamp, null), i);
        }

        // Use ATR from the compound hub's input
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

        // Rebuild windows from the quote provider's cache
        int index = _quoteProvider.Results.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        // Rebuild up to the index before the rollback timestamp
        int targetIndex = index - 1;
        int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);

        for (int p = startIdx; p <= targetIndex; p++)
        {
            IQuote quote = _quoteProvider.Results[p];
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

    /// <summary>
    /// Creates a new Chandelier Exit hub, using ATR values from an existing ATR hub.
    /// </summary>
    /// <param name="atrHub">The existing ATR hub.</param>
    /// <param name="quoteProvider">The quote provider (must be the same provider used by the ATR hub).</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier to apply to the ATR.</param>
    /// <param name="type">The type of Chandelier Exit to calculate (Long or Short).</param>
    /// <returns>An instance of <see cref="ChandelierHub"/>.</returns>
    /// <remarks>
    /// <para>IMPORTANT: This is not a normal chaining approach.</para>
    /// This extension overrides the standard chaining pattern to specifically
    /// reuse an existing <see cref="AtrHub"/> internally, avoiding duplicate ATR calculations
    /// when multiple indicators need the same ATR values.
    /// The quote provider must be the same provider used to create the ATR hub;
    /// providing a different provider may result in index misalignment and incorrect calculations.
    /// </remarks>
    public static ChandelierHub ToChandelierHub(
        this AtrHub atrHub,
        IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 22,
        double multiplier = 3,
        Direction type = Direction.Long)
             => new(atrHub, quoteProvider, lookbackPeriods, multiplier, type);
}
