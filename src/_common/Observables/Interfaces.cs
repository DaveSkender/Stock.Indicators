namespace Skender.Stock.Indicators;

// PUBLIC INTERFACES ONLY *****
// Reminder: do not add non-public elements here for internal templating.

/// <summary>
/// Observable provider incremental quotes from external feed sources.
/// </summary>
/// <typeparam name="TQuote" cref="IQuote">
///   Quote based on IQuote interface
/// </typeparam>
public interface IQuoteProvider<TQuote>
    : IStreamCache<TQuote>, IObservable<(Act act, TQuote quote)>
    where TQuote : IQuote
{
    /// <summary>
    /// Add a single quote.  We'll determine if it's new or an update.
    /// </summary>
    /// <param name="quote">Quote to add or update</param>
    Act Add(TQuote quote);

    /// <summary>
    /// Delete a quote.  We'll double-check that it exists in the
    /// cache before propogating the event to subscribers.
    /// </summary>
    /// <param name="quote">Quote to delete</param>
    Act Delete(TQuote quote);

    /// <summary>
    /// Terminates all subscriber connections gracefully.
    /// </summary>
    void EndTransmission();
}

/// <summary>
/// Observable provider of incremental chain value changes.
/// </summary>
public interface IChainProvider
    : IObservable<(Act act, DateTime date, double price)>
{
    /// <summary>
    /// Terminates all subscriber connections gracefully.
    /// </summary>
    void EndTransmission();
}

/// <summary>
/// Observer of incremental quotes from external feed sources.
/// </summary>
/// <typeparam name="TQuote"></typeparam>
public interface IQuoteObserver<TQuote>
    : IObserver<(Act act, TQuote quote)>
    where TQuote : IQuote
{
    /// <summary>
    /// Unsubscribe from the data provider
    /// </summary>
    void Unsubscribe();

    /// <summary>
    /// Reset the entire results cache and rebuild it from quote provider sources.
    /// Consider using RebuildCache(fromTimestapmp) from a known point in time instead.
    /// </summary>
    void RebuildCache();

    /// <summary>
    /// Reset and rebuild the results cache from a point in time.
    /// Use RebuildCache() without arguments to reset the entire cache.
    /// </summary>
    /// <param name="fromTimestamp"></param>
    void RebuildCache(DateTime fromTimestamp);
}

/// <summary>
/// Observer of indicator chain value changes.
/// </summary>
/// <typeparam name="TResult" cref="IResult"></typeparam>
public interface IChainObserver<TResult>
    : IStreamCache<TResult>, IObserver<(Act act, DateTime date, double price)>
    where TResult : IResult
{
    /// <summary>
    /// Unsubscribe from the data provider
    /// </summary>
    void Unsubscribe();

    /// <summary>
    /// Reset the entire results cache and rebuild it from provider sources.
    /// Consider using RebuildCache(fromTimestapmp) from a known point in time instead.
    /// </summary>
    void RebuildCache();

    /// <summary>
    /// Reset and rebuild the results cache from a point in time.
    /// Use RebuildCache() without arguments to reset the entire cache.
    /// </summary>
    /// <param name="fromTimestamp"></param>
    void RebuildCache(DateTime fromTimestamp);
}

/// <summary>
/// Read-only quote or indicator values.  They are automatically updated.
/// </summary>
/// <typeparam name="TSeries" cref="ISeries"></typeparam>
public interface IStreamCache<TSeries>
    where TSeries : ISeries
{
    /// <summary>
    /// Read-only quote or indicator time-series values.
    /// They are automatically updated.
    /// </summary>
    IEnumerable<TSeries> Results { get; }

    /// <summary>
    /// Returns a short formatted label with parameter values, e.g. EMA(10)
    /// </summary>
    /// <returns>Indicator or quote label</returns>
    string ToString();

    /// <summary>
    /// Deletes all cached time-series records, without restore.
    /// Subscribed indicators' caches will also be deleted.
    /// </summary>
    void ClearCache();

    /// <summary>
    /// Deletes cached time-series records from point in time, without restore.
    /// Subscribed indicators' caches will also be deleted accordingly.
    /// </summary>
    /// <param name="fromTimestamp"></param>
    void ClearCache(DateTime fromTimestamp);
}
