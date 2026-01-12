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
    private bool _isStandalone;

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
    /// Subscribes an observer to this QuoteHub.
    /// </summary>
    /// <param name="observer">The observer to subscribe.</param>
    /// <returns>A disposable subscription token.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to subscribe a standalone QuoteHub as an observer.
    /// </exception>
    /// <remarks>
    /// Standalone QuoteHubs cannot be subscribed to other providers after construction
    /// because they manage their own data without an external provider.
    /// To create a QuoteHub that observes another provider, use the constructor
    /// <c>new QuoteHub(provider)</c> or the extension method <c>provider.ToQuoteHub()</c>.
    /// </remarks>
    public override IDisposable Subscribe(IStreamObserver<IQuote> observer)
    {
        // Check if the observer being added is a standalone QuoteHub
        if (observer is QuoteHub quoteHub && quoteHub._isStandalone)
        {
            const string msg = """
                Cannot subscribe a standalone QuoteHub to another provider.
                Standalone QuoteHubs are created without a provider using 'new QuoteHub()' and manage their own data.
                To create a QuoteHub that subscribes to a provider, use one of these approaches:
                  - Constructor: new QuoteHub(provider)
                  - Extension method: provider.ToQuoteHub()
                """;

            throw new InvalidOperationException(msg);
        }

        return base.Subscribe(observer);
    }

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
/// </summary>
/// <typeparam name="T">Type of record</typeparam>
public class BaseProvider<T>
    : IStreamObservable<T>
    where T : IReusable
{
    /// <summary>
    /// Inert quote provider is parent-less Hub.
    /// It does not transfer its setting to its children.
    /// </summary>
    public BinarySettings Properties { get; } = new(0b00000001, 0b11111110);

    /// <inheritdoc/>
    public int MaxCacheSize => 0;

    /// <summary>
    /// Gets the number of observers.
    /// </summary>
    public int ObserverCount => 0;

    /// <summary>
    /// Gets a value indicating whether there are any observers.
    /// </summary>
    public bool HasObservers => false;

    /// <summary>
    /// Gets the list of quotes.
    /// </summary>
    public IReadOnlyList<T> Quotes { get; } = Array.Empty<T>();

    /// <summary>
    /// Gets a reference to the cache.
    /// </summary>
    /// <returns>A read-only list of quotes.</returns>
    public IReadOnlyList<T> GetCacheRef() => Array.Empty<T>();

    /// <summary>
    /// Determines whether the specified observer is a subscriber.
    /// </summary>
    /// <param name="observer">The observer to check.</param>
    /// <returns><c>true</c> if the observer is a subscriber; otherwise, <c>false</c>.</returns>
    public bool HasSubscriber(IStreamObserver<T> observer) => false;

    /// <summary>
    /// Subscribes the specified observer.
    /// </summary>
    /// <param name="observer">The observer to subscribe.</param>
    /// <returns>A disposable object that can be used to unsubscribe.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the operation is invalid for the current state</exception>
    public IDisposable Subscribe(IStreamObserver<T> observer)
        => throw new InvalidOperationException();

    /// <summary>
    /// Unsubscribes the specified observer.
    /// </summary>
    /// <param name="observer">The observer to unsubscribe.</param>
    /// <returns><c>true</c> if the observer was unsubscribed; otherwise, <c>false</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the operation is invalid for the current state</exception>
    public bool Unsubscribe(IStreamObserver<T> observer)
        => throw new InvalidOperationException();

    /// <summary>
    /// Ends the transmission.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the operation is invalid for the current state</exception>
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
