namespace Skender.Stock.Indicators;

// PUBLIC INTERFACES ONLY *****
// Reminder: do not add non-public elements here for internal templating.
// Conversly, non-public members should be defined as internal or private.

/// <summary>
/// Observable provider incremental quotes from external feed sources.
/// </summary>
public interface IQuoteProvider
    : IObservable<(Act act, IQuote quote)>
{
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
public interface IQuoteObserver<TResult>
    : IStreamCache<TResult>, IObserver<(Act act, IQuote quote)>
    where TResult : IResult
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
    /// <param name="fromTimestamp">
    ///   All periods (inclusive) after this DateTime will
    ///   be removed and recalculated.
    /// </param>
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
    /// <param name="fromTimestamp">
    ///   All periods (inclusive) after this DateTime will
    ///   be removed and recalculated.
    /// </param>
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
    /// <param name="fromTimestamp">
    ///   All periods (inclusive) after this DateTime will be removed.
    /// </param>
    void ClearCache(DateTime fromTimestamp);
}
