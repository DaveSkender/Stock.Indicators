namespace Skender.Stock.Indicators;

// STREAM OBSERVABLE INTERFACES (PROVIDERS)
// with Quote, Chain, Result variants (at bottom)

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
    /// Returns a short text label
    /// with parameter values, e.g. EMA(10)
    /// </summary>
    /// <returns>String label</returns>
    string ToString();

    /// <summary>
    /// Unsubscribe all observers (subscribers)
    /// </summary>
    void EndTransmission();

    /// <summary>
    /// Finds cache index position of the provided timestamp
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns>Index value or -1 when not found</returns>
    int FindIndex(DateTime timeStamp);

    /// <summary>
    /// Resends all newer values to a requesting observer,
    /// starting from a specific timestamp.
    /// </summary>
    /// <param name="toObserver">Subscribtion identity</param>
    /// <param name="fromTimestamp">From timestamp, inclusive</param>
    void Resend(
        IObserver<(Act, TSeries)> toObserver,
        DateTime fromTimestamp);

    /// <summary>
    /// Resends all newer values to a requesting observer,
    /// starting at an index position.
    /// </summary>
    /// <param name="toObserver">Subscribtion identity</param>
    /// <param name="fromIndex">From index, inclusive</param>
    void Resend(
        IObserver<(Act, TSeries)> toObserver,
        int fromIndex);

    /// <summary>
    /// Resends all values in a range to a requesting observer,
    /// starting and ending at an index position.
    /// </summary>
    /// <param name="toObserver">Subscribtion identity</param>
    /// <param name="fromIndex">Starting index, inclusive</param>
    /// <param name="toIndex">Ending index, inclusive</param>
    void Resend(
        IObserver<(Act, TSeries)> toObserver,
        int fromIndex,
        int toIndex);
}

#region QUOTE, CHAIN, RESULT PROVIDERS

/// <summary>
/// Quote provider interface (observable)
/// </summary>
/// <typeparam name="TQuote"></typeparam>
public interface IQuoteProvider<TQuote> : IStreamProvider<TQuote>
    where TQuote : struct, IQuote;

/// <summary>
/// Chainable provider interface (observable)
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IChainProvider<TResult> : IStreamProvider<TResult>
    where TResult : struct, IReusable;

/// <summary>
/// Non-chainable provider interface (observable)
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IResultProvider<TResult> : IStreamProvider<TResult>
    where TResult : struct, IResult;
#endregion
