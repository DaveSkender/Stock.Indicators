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
    /// Initializes a new instance of the <see cref="QuoteHub"/> base, without its own provider.
    /// </summary>
    /// <param name="maxCacheSize">Maximum in-memory cache size.</param>
    public QuoteHub(int? maxCacheSize = null)
        : base(new BaseProvider<IQuote>())
    {
        _isStandalone = true;

        const int maxCacheSizeDefault = (int)(0.9 * int.MaxValue);

        if (maxCacheSize is not null and > maxCacheSizeDefault)
        {
            string message
                = $"'{nameof(maxCacheSize)}' must be less than {maxCacheSizeDefault}.";

            throw new ArgumentOutOfRangeException(
                nameof(maxCacheSize), maxCacheSize, message);
        }

        MaxCacheSize = maxCacheSize ?? maxCacheSizeDefault;
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
        // for non-standalone QuoteHub, use standard behavior
        if (!_isStandalone)
        {
            base.OnAdd(item, notify, indexHint);
            return;
        }

        // get result and position
        (IQuote result, int index) = ToIndicator(item, indexHint);

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

            // notify observers to rebuild from this timestamp
            if (notify)
            {
                NotifyObserversOnRebuild(result.Timestamp);
            }

            return;
        }

        // standard add behavior for new items
        AppendCache(result, notify);
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
            // rollback internal state
            RollbackState(fromTimestamp);

            // notify observers to rebuild from this hub
            NotifyObserversOnRebuild(fromTimestamp);
            return;
        }

        // standard rebuild for QuoteHub with external provider
        base.Rebuild(fromTimestamp);
    }
}

/// <summary>
/// Inert provider for base Hub initialization.
/// It has no upstream data and cannot be observed.
/// </summary>
/// <typeparam name="T">Type of record</typeparam>
public class BaseProvider<T>
    : IStreamObservable<T>
    where T : IReusable
{
    /// <summary>
    /// Hub properties with non-standard defaults:
    /// bit 0 = 1 (disable observer) and mask = 0b11111110 (do not pass bit 0 to child hubs).
    /// </summary>
    /// <remarks>
    /// <see cref="BaseProvider{T}"/> is an inert provider that cannot observe.
    /// Bit 0 is set to 1 (disable observer) and masked to prevent child hubs from inheriting this restriction,
    /// allowing downstream hubs to be proper observers even though the base provider is not.
    /// <para>See <see cref="BinarySettings"/> for more information on bit settings and masks.</para>
    /// </remarks>
    public BinarySettings Properties { get; } = new(0b00000001, 0b11111110);

    /// <inheritdoc/>
    public int MaxCacheSize => 0;

    /// <inheritdoc />
    public int ObserverCount => 0;

    /// <inheritdoc />
    public bool HasObservers => false;

    /// <inheritdoc />
    public IReadOnlyList<T> GetCacheRef() => Array.Empty<T>().AsReadOnly();

    /// <inheritdoc />
    public bool HasSubscriber(IStreamObserver<T> observer) => false;

    /// <inheritdoc />
    public IDisposable Subscribe(IStreamObserver<T> observer)
        => throw new InvalidOperationException();

    /// <inheritdoc />
    public bool Unsubscribe(IStreamObserver<T> observer)
        => throw new InvalidOperationException();

    /// <inheritdoc />
    public void EndTransmission()
        => throw new InvalidOperationException();
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
