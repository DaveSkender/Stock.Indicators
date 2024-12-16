namespace Skender.Stock.Indicators;

#region hub initializer

public static partial class Quotes
{
    /// <summary>
    /// Converts an IQuoteProvider to a QuoteHub.
    /// </summary>
    /// <typeparam name="TQuote">The type of quote.</typeparam>
    /// <param name="quoteProvider">The quote provider to convert.</param>
    /// <returns>A new instance of QuoteHub.</returns>
    public static QuoteHub<TQuote> ToQuote<TQuote>(
        this IQuoteProvider<TQuote> quoteProvider)
        where TQuote : IQuote => new(quoteProvider);
}
#endregion

/// <summary>
/// Represents a hub for managing quotes.
/// </summary>
/// <typeparam name="TQuote">The type of quote.</typeparam>
public class QuoteHub<TQuote>
    : QuoteProvider<TQuote, TQuote>
    where TQuote : IQuote
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteHub{TQuote}"/> base, without its own provider.
    /// </summary>
    /// <param name="maxCacheSize">Maximum in-memory cache size.</param>
    public QuoteHub(int? maxCacheSize = null)
        : base(new BaseProvider<TQuote>())
    {

        const int maxCacheSizeDefault = (int)(0.9 * int.MaxValue);

        if (maxCacheSize != null && maxCacheSize > maxCacheSizeDefault)
        {
            string message
                = $"'{nameof(maxCacheSize)}' must be less than {maxCacheSizeDefault}.";

            throw new ArgumentOutOfRangeException(
                nameof(maxCacheSize), maxCacheSize, message);
        }

        MaxCacheSize = maxCacheSize ?? maxCacheSizeDefault;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteHub{TQuote}"/> class with a specified provider.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    public QuoteHub(
        IQuoteProvider<TQuote> provider) : base(provider)
    {
        Reinitialize();
    }

    // METHODS

    /// <inheritdoc/>
    protected override (TQuote result, int index)
        ToIndicator(TQuote item, int? indexHint)
    {
        int index = indexHint
            ?? Cache.IndexGte(item.Timestamp);

        return (item, index == -1 ? Cache.Count : index);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"QUOTES<{typeof(TQuote).Name}>: {Quotes.Count} items";
}

/// <summary>
/// Inert provider for base Hub initialization.
/// </summary>
/// <typeparam name="T"></typeparam>
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
    public IDisposable Subscribe(IStreamObserver<T> observer)
        => throw new InvalidOperationException();

    /// <summary>
    /// Unsubscribes the specified observer.
    /// </summary>
    /// <param name="observer">The observer to unsubscribe.</param>
    /// <returns><c>true</c> if the observer was unsubscribed; otherwise, <c>false</c>.</returns>
    public bool Unsubscribe(IStreamObserver<T> observer)
        => throw new InvalidOperationException();

    /// <summary>
    /// Ends the transmission.
    /// </summary>
    public void EndTransmission()
        => throw new InvalidOperationException();
}
