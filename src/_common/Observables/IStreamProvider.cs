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
    /// Resends historical cached values to a requesting observer,
    /// starting at an index position.
    /// </summary>
    /// <param name="fromIndex"></param>
    /// <param name="toObserver"></param>
    void Resend(int fromIndex, IObserver<(Act, TSeries)> toObserver);

    /// <summary>
    /// Finds the index position of the provided timestamp
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns>Index value or -1 when not found</returns>
    int FindIndex(DateTime timeStamp);
}
