namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Commodity Channel Index (CCI) calculations.
/// </summary>
public class CciHub
    : ChainProvider<IQuote, CciResult>, ICci
{
    private readonly string hubName;
    private readonly CciList _cciList;

    /// <summary>
    /// Initializes a new instance of the <see cref="CciHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal CciHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        Cci.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"CCI({lookbackPeriods})";
        _cciList = new CciList(lookbackPeriods);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (CciResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Add current item to CciList
        _cciList.Add(item);

        // Get the latest result from the CciList
        CciResult r = _cciList[^1];

        return (r, i);
    }

    /// <summary>
    /// Restores the CciList state up to the specified timestamp.
    /// Clears and rebuilds _cciList from ProviderCache for Insert/Remove operations.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear CciList
        _cciList.Clear();

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

        // Optimize: only rebuild the rolling window needed for CciList
        // CciList maintains a _tpBuffer of size LookbackPeriods via Queue.Update()
        int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);

        // Rebuild CciList from ProviderCache
        for (int p = startIdx; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];
            _cciList.Add(quote);
        }
    }
}

/// <summary>
/// Provides methods for calculating the Commodity Channel Index (CCI) indicator.
/// </summary>
public static partial class Cci
{
    /// <summary>
    /// Creates a CCI hub from a quote provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A CCI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static CciHub ToCciHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 20)
        => new(quoteProvider, lookbackPeriods);

    /// <summary>
    /// Creates a CCI hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An instance of <see cref="CciHub"/>.</returns>
    public static CciHub ToCciHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToCciHub(lookbackPeriods);
    }
}
