namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for the Money Flow Index (MFI) indicator.
/// </summary>
public class MfiHub : ChainProvider<IQuote, MfiResult>, IMfi
{
    private readonly string hubName;
    private readonly MfiList _mfiList;

    /// <summary>
    /// Initializes a new instance of the <see cref="MfiHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal MfiHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods)
        : base(provider)
    {
        Mfi.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        hubName = $"MFI({lookbackPeriods})";
        _mfiList = new MfiList(lookbackPeriods);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (MfiResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Add current item to MfiList
        _mfiList.Add(item);

        // Get the latest result from the MfiList
        MfiResult r = _mfiList[^1];

        return (r, i);
    }

    /// <summary>
    /// Restores the MfiList state up to the specified timestamp.
    /// Clears and rebuilds _mfiList from ProviderCache for Insert/Remove operations.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear MfiList
        _mfiList.Clear();

        // Find target index in ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index == -1)
        {
            index = ProviderCache.Count;
        }
        if (index <= 0)
        {
            return;
        }

        // Rebuild up to the index before the rollback timestamp
        int targetIndex = index - 1;

        // Optimize: only rebuild the rolling window needed for MfiList
        // MfiList maintains a _buffer of size LookbackPeriods via Queue.Update()
        int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);

        // Rebuild MfiList from ProviderCache
        for (int p = startIdx; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];
            _mfiList.Add(quote);
        }
    }
}


public static partial class Mfi
{
    /// <summary>
    /// Converts the quote provider to an MFI hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods. Default is 14.</param>
    /// <returns>An MFI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static MfiHub ToMfiHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 14)
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider, lookbackPeriods);
    }

    /// <summary>
    /// Creates an Mfi hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of lookback periods. Default is 14.</param>
    /// <returns>An instance of <see cref="MfiHub"/>.</returns>
    public static MfiHub ToMfiHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToMfiHub(lookbackPeriods);
    }
}
