namespace Skender.Stock.Indicators;

// PROVIDER INTERFACES (OBSERVABLES)

/// <summary>
/// Quote provider interface (observable)
/// </summary>
/// <typeparam name="TQuote"></typeparam>
public interface IQuoteProvider<TQuote> : IStreamProvider<TQuote>
    where TQuote : struct, IQuote;

/// <summary>
/// Chainable result provider interface (observable)
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IChainProvider<TResult> : IStreamProvider<TResult>
    where TResult : struct, IReusableResult;

/// <summary>
/// Non-chainable result provider interface (observable)
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IResultProvider<TResult> : IStreamProvider<TResult>
    where TResult : struct, IResult;

/// <summary>
/// Streaming provider interface (observable)
/// </summary>
/// <typeparam name="TSeries"></typeparam>
public interface IStreamProvider<TSeries> : IObservable<(Act, TSeries)>
    where TSeries : struct, ISeries
{
    /// <summary>
    /// Currently has subscribers
    /// </summary>
    bool HasSubscribers { get; }

    /// <summary>
    /// Current number of subscribers
    /// </summary>
    int SubscriberCount { get; }

    /// <summary>
    /// Read-only cache of historical provider cache values
    /// </summary>
    IReadOnlyList<TSeries> Results { get; }

    /// <summary>
    /// Returns a short formatted label
    /// with parameter values, e.g. EMA(10)
    /// </summary>
    /// <returns>Indicator or quote label</returns>
    string ToString();

    /// <summary>
    /// Unsubscribe all observers (subscribers)
    /// </summary>
    void EndTransmission();

    /// <summary>
    /// Finds the index position in the cache, of the provided timestamp
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns>Index value or -1 when not found</returns>
    int FindIndex(DateTime timeStamp);

    /// <summary>
    /// Resends historical cached values to a requesting observer,
    /// starting from a specific timestamp.
    /// </summary>
    /// <param name="toObserver">Subscriber identity.</param>
    /// <param name="fromTimestamp">First period to resend.</param>
    void Resend(
        IObserver<(Act, TSeries)> toObserver,
        DateTime fromTimestamp);

    /// <summary>
    /// Resends historical cached values to a requesting observer,
    /// starting at an index position.
    /// </summary>
    /// <param name="toObserver">Subscriber identity.</param>
    /// <param name="fromIndex">First periods to resend.</param>
    /// <param name="toIndex">
    /// The last period to include, or all (default).
    /// </param>
    void Resend(
        IObserver<(Act, TSeries)> toObserver,
        int fromIndex,
        int? toIndex = null);
}
