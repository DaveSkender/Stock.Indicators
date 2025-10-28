namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for managing quotes.
/// </summary>
public class QuoteHub
    : QuoteProvider<IQuote, IQuote>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteHub"/> base, without its own provider.
    /// </summary>
    /// <param name="maxCacheSize">Maximum in-memory cache size.</param>
    public QuoteHub(int? maxCacheSize = null)
        : base(new BaseProvider<IQuote>())
    {

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
        Reinitialize();
    }

    // METHODS

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

    /// <summary>
    /// Creates a new QuoteHub from an initiating collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public static QuoteHub ToQuoteHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();  // cannot dogfood ToQuoteHub() here
        quoteHub.Add(quotes);
        return quoteHub;
    }
}
