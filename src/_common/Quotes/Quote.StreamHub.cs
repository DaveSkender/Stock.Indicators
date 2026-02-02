namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for managing quotes.
/// </summary>
public class QuoteHub
    : QuoteProvider<IQuote, IQuote>
{
    /// <summary>
    /// Indicates whether this QuoteHub is standalone (no external provider).
    /// </summary>
    private readonly bool _isStandalone;

    /// <summary>
    /// Absolute maximum cache size to prevent overflow.
    /// </summary>
    private const int absoluteMaxCacheSize = (int)(0.8 * int.MaxValue);

    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteHub"/> base, without its own provider.
    /// </summary>
    /// <param name="maxCacheSize">Maximum in-memory cache size.</param>
    public QuoteHub(int? maxCacheSize = null)
        : base(new BaseProvider<IQuote>(ValidateAndGetMaxCacheSize(maxCacheSize)))
    {
        _isStandalone = true;
    }

    /// <summary>
    /// Validates and returns the max cache size.
    /// </summary>
    /// <param name="maxCacheSize">Maximum in-memory cache size.</param>
    /// <returns>Validated max cache size.</returns>
    private static int ValidateAndGetMaxCacheSize(int? maxCacheSize)
    {
        const int maxCacheSizeDefault = 100_000;

        if (maxCacheSize is (not null and <= 0) or > absoluteMaxCacheSize)
        {
            string message
                = $"'{nameof(maxCacheSize)}' must be greater than 0 and not over {absoluteMaxCacheSize}.";

            throw new ArgumentOutOfRangeException(
                nameof(maxCacheSize), maxCacheSize, message);
        }

        return maxCacheSize ?? maxCacheSizeDefault;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteHub"/> class with a specified provider.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    public QuoteHub(
        IQuoteProvider<IQuote> provider)
        : base(provider)
    {
        _isStandalone = false;
        Reinitialize();
    }

    /// <inheritdoc/>
    protected override (IQuote result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int index = indexHint
            ?? Cache.IndexGte(item.Timestamp);

        return (item, index == -1 ? Cache.Count : index);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"QUOTES<{nameof(IQuote)}>: {Quotes.Count} items";

    /// <summary>
    /// Handles adding a new quote with special handling for same-timestamp updates
    /// when QuoteHub is standalone (no external provider).
    /// </summary>
    /// <inheritdoc/>
    public override void OnAdd(IQuote item, bool notify, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        // Reject additions that precede the current cache timeline
        // (applies to both standalone and non-standalone QuoteHub)
        lock (CacheLock)
        {
            if (Cache.Count > 0 && item.Timestamp < Cache[0].Timestamp)
            {
                // Silently ignore - this prevents indeterminate gaps in the timeline
                return;
            }
        }

        // for non-standalone QuoteHub, use standard behavior (which handles locking)
        if (!_isStandalone)
        {
            base.OnAdd(item, notify, indexHint);
            return;
        }

        // Lock for standalone QuoteHub operations
        lock (CacheLock)
        {

            // get result and position
            (IQuote result, int index) = ToIndicator(item, indexHint);

            // Reject modifications that would affect indices before MinCacheSize
            // to prevent corrupted rebuilds in subscribers
            // This includes both insertions and same-timestamp replacements
            if (index >= 0 && index < MinCacheSize && index < Cache.Count)
            {
                // Silently ignore all modifications before MinCacheSize
                return;
            }

            // check if this is a same-timestamp update (not a new item at the end)
            if (Cache.Count > 0 && index < Cache.Count && Cache[index].Timestamp == result.Timestamp)
            {
                // check if this is an exact duplicate (same values)
                // if so, defer to AppendCache for overflow tracking
                if (Cache[index].Equals(result))
                {
                    AppendCache(result, notify);
                    return;
                }

                // replace existing item at this position (different values, same timestamp)
                Cache[index] = result;

                // notify observers (inside lock to ensure cache consistency)
                if (notify)
                {
                    NotifyObserversOnRebuild(item.Timestamp);
                }
            }
            else
            {
                // if out-of-order insert, avoid rebuilding this hub when standalone
                if (index >= 0 && index < Cache.Count)
                {
                    InsertWithoutRebuild(result, index, notify);
                    return;
                }

                // standard add behavior for new items
                AppendCache(result, notify);
            }
        }
    }

    /// <summary>
    /// Rebuilds the cache from a specific timestamp.
    /// For standalone QuoteHub, preserves cache and notifies observers.
    /// </summary>
    /// <inheritdoc/>
    public override void Rebuild(DateTime fromTimestamp)
    {
        // for standalone QuoteHub (no external provider),
        // we cannot rebuild from an empty provider cache
        // instead, just notify observers to rebuild from this hub's cache
        if (_isStandalone)
        {
            lock (CacheLock)
            {
                // rollback internal state
                RollbackState(fromTimestamp);

                // notify observers to rebuild from this hub (inside lock
                // to ensure cache consistency before any new items are added)
                NotifyObserversOnRebuild(fromTimestamp);
            }

            return;
        }

        // standard rebuild for QuoteHub with external provider
        base.Rebuild(fromTimestamp);
    }
}

public static partial class Quotes
{
    /// <summary>
    /// Creates a QuoteHub that is subscribed to an IQuoteProvider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider to convert.</param>
    /// <returns>A new instance of QuoteHub.</returns>
    public static QuoteHub ToQuoteHub(
        this IQuoteProvider<IQuote> quoteProvider)
        => new(quoteProvider);
}
