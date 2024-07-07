namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub that manages its cache.
/// It is the base for other, more-specific streaming hub types.
/// </summary>
/// <typeparam name="TOut"></typeparam>
public interface IStreamHub<TOut>
    where TOut : struct, ISeries
{
    /// <summary>
    /// Returns a short text label
    /// with parameter values, e.g. "EMA(10)"
    /// </summary>
    /// <returns>String label</returns>
    string ToString();

    /// <inheritdoc cref="IStreamCache" />
    StreamCache<TOut> StreamCache { get; }
}

/// <summary>
/// Streaming hub that manages its cache, observers, and observables.
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public interface IObserverHub<TIn, TOut> : IStreamHub<TOut>
    where TIn : struct, ISeries
    where TOut : struct, ISeries
{
    /// <summary>
    /// Handles new data from <see cref="StreamProvider{TSeries}"/>.
    /// </summary>
    /// <param name="newItem">New value from provider</param>
    void OnNextNew(TIn newItem); // TODO: shouldn't this really just be "Add"?

    /// <summary>
    /// Read-only cache of processed results
    /// </summary>
    IReadOnlyList<TOut> Results { get; }  // TODO: are the objects in this list editable?

    /// <summary>
    /// Supplying provider for this hub (observable)
    /// </summary>
    /// <inheritdoc cref="IStreamProvider{TSeries}" />
    StreamProvider<TIn> Supplier { get; }

    // resurfaced methods

    /// <inheritdoc cref="IStreamObserver{TIn}" />
    StreamObserver<TIn, TOut> Observer { get; }

    /// <inheritdoc cref="IStreamObserver{TIn}.Unsubscribe" />
    void Unsubscribe();

    /// <inheritdoc cref="IStreamObserver{TIn}.Reinitialize()"/>
    void Reinitialize();

    /// <inheritdoc cref="IStreamObserver{TIn}.RebuildCache()"/>
    void RebuildCache();

    /// <inheritdoc cref="IStreamObserver{TIn}.RebuildCache(DateTime)"/>
    void RebuildCache(DateTime fromTimestamp);

    /// <inheritdoc cref="IStreamObserver{TIn}.RebuildCache(int)"/>
    void RebuildCache(int fromIndex);

    ///// <inheritdoc cref="IStreamProvider{TSeries}.ClearCache()"/>
    //void ClearCache();

    ///// <inheritdoc cref="IStreamProvider{TSeries}.ClearCache(DateTime)"/>
    //void ClearCache(DateTime fromTimestamp);

    ///// <inheritdoc cref="IStreamProvider{TSeries}.ClearCache(int)"/>
    //void ClearCache(int fromIndex);
}

/// <summary>
/// Streaming quote hub that manages its cache and its observable role.
/// It can be a sole provider or a redistirubiton subscriber for other quote hubs.
/// </summary>
/// <typeparam name="TQuote"></typeparam>
public interface IQuoteHub<TQuote> : IObserverHub<TQuote, TQuote>
    where TQuote : struct, IQuote
{
    /// <inheritdoc cref="StreamCache{TQuote}.Cache" />
    IReadOnlyList<TQuote> Quotes { get; }

    /// <summary>
    /// Add a single quote.
    /// We'll determine if it's new or an update.
    /// </summary>
    /// <param name="quote" cref="IQuote">
    /// Quote to add or update
    /// </param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    Act Add(TQuote quote);

    /// <summary>
    /// Add a batch of quotes.
    /// We'll determine if they're new or updated.
    /// </summary>
    /// <param name="quotes" cref="IQuote">
    ///   Batch of quotes to add or update
    /// </param>
    void Add(IEnumerable<TQuote> quotes);

    /// <summary>
    /// Delete a quote.  We'll double-check that it exists in the
    /// cache before propogating the event to subscribers.
    /// </summary>
    /// <param name="quote">Quote to delete</param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    Act Delete(TQuote quote);
}
