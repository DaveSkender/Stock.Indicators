namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Commodity Channel Index (CCI) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class CciHub<TIn>
    : ChainProvider<TIn, CciResult>, ICci
    where TIn : IQuote
{
    private readonly string hubName;
    private readonly CciList _cciList;

    /// <summary>
    /// Initializes a new instance of the <see cref="CciHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal CciHub(
        IQuoteProvider<TIn> provider,
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
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Synchronize _cciList state with the current index
        if (i == 0)
        {
            // Starting fresh - clear and add first item
            _cciList.Clear();
            _cciList.Add(item);
        }
        else if (_cciList.Count == i)
        {
            // Normal incremental case - list is in sync, just add the new item
            _cciList.Add(item);
        }
        else if (_cciList.Count < i)
        {
            // Missing data - add any skipped items from ProviderCache, then add current
            for (int k = _cciList.Count; k < i; k++)
            {
                _cciList.Add(ProviderCache[k]);
            }

            _cciList.Add(item);
        }
        else // _cciList.Count > i
        {
            // Late arrival/rebuild scenario - items were removed/reordered
            // Must rebuild from scratch to ensure correctness
            _cciList.Clear();
            for (int k = 0; k < i; k++)
            {
                _cciList.Add(ProviderCache[k]);
            }

            _cciList.Add(item);
        }

        // Get the latest result from the CciList
        CciResult r = _cciList[^1];

        return (r, i);
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
    /// <typeparam name="T">The type of the quote data.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A CCI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static CciHub<T> ToCci<T>(
        this IQuoteProvider<T> quoteProvider,
        int lookbackPeriods = 20)
        where T : IQuote
        => new(quoteProvider, lookbackPeriods);

    /// <summary>
    /// Creates a CCI hub from a collection of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An instance of <see cref="CciHub{TQuote}"/>.</returns>
    public static CciHub<TQuote> ToCci<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 20)
        where TQuote : IQuote
    {
        QuoteHub<TQuote> quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToCci(lookbackPeriods);
    }
}
