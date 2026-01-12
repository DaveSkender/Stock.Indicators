namespace Skender.Stock.Indicators;

// TODO: rename to BaseObservable
// or determine if this can/should be entirely removed.
// It is only used as a workaround for initializing QuoteHub base class without a provider.

/// <summary>
/// Inert provider for base Hub initialization.
/// It has no upstream data and cannot be observed.
/// </summary>
/// <typeparam name="T">Type of record</typeparam>
/// <remarks>
/// Only used to initialize a <see cref="QuoteHub"/> base that does not have its own provider.
/// </remarks>
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

    /// <inheritdoc />
    /// <remarks>
    /// <see cref="BaseProvider{T}"/> does not have cached values."
    /// </remarks>
    public IReadOnlyList<T> ReadCache => Array.Empty<T>().AsReadOnly();

    /// <inheritdoc/>
    public int MaxCacheSize => 0;

    /// <inheritdoc />
    public int ObserverCount => 0;

    /// <inheritdoc />
    public bool HasObservers => false;

    /// <inheritdoc />
    public bool HasSubscriber(IStreamObserver<T> observer) => false;

    /// <inheritdoc />
    public IDisposable Subscribe(IStreamObserver<T> observer)
        => throw new InvalidOperationException("Subscriptions not allowed on base provider.");

    /// <inheritdoc />
    public bool Unsubscribe(IStreamObserver<T> observer)
        => throw new InvalidOperationException("Unsubscribe has no effect on base provider.");

    /// <inheritdoc />
    public void EndTransmission()
        => throw new InvalidOperationException("Base provider does not transmit to subscribers.");
}
