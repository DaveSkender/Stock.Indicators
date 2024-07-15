namespace Skender.Stock.Indicators;

// STREAM HUB INTERFACES

#region quote, reusable, & result out types

public interface IQuoteHub<TIn, TOut>
    : IStreamHub<TIn, TOut>, IQuoteProvider<TOut>, IChainProvider<TOut>
    where TIn : IQuote
    where TOut : IQuote;

/// <inheritdoc />
public interface IReusableHub<TIn, TOut>
    : IStreamHub<TIn, TOut>, IChainProvider<TOut>
    where TIn : ISeries
    where TOut : IReusable;

/// <inheritdoc />
public interface IResultHub<TIn, TOut>
    : IStreamHub<TIn, TOut>
    where TIn : ISeries
    where TOut : ISeries;
#endregion

/// <summary>
/// Streaming hub (observer and observable provider).
/// </summary>
public interface IStreamHub<TIn, TOut>
    : IObserver<(Act act, TIn inbound)>, IStreamProvider<TOut>
    where TIn : ISeries
    where TOut : ISeries
{
    /// <summary>
    /// Current state of subscription to provider.
    /// </summary>
    bool IsSubscribed { get; }

    /// <summary>
    /// Unsubscribe from the data provider.
    /// </summary>
    void Unsubscribe();

    /// <summary>
    /// Full reset of the provider subscription.
    /// </summary>
    /// <remarks>
    /// This unsubscribes from the provider,
    /// clears cache, cascading deletes to subscribers,
    /// then re-subscribes to the provider (with rebuild).
    /// <para>
    /// This is also used on startup to invoke provider
    /// <see cref="IObservable{T}.Subscribe(IObserver{T})"/>.
    /// </para>
    /// </remarks>
    void Reinitialize();

    /// <summary>
    /// Reset the entire results cache
    /// and rebuild it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <remarks>
    /// This is different from <see cref="Reinitialize()"/>.
    /// It does not reset the provider subscription.
    /// </remarks>
    void RebuildCache();

    /// <summary>
    /// Reset the entire results cache from a point in time
    /// and rebuilds it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromTimestamp">
    /// All periods (inclusive) after this date/time will
    /// be removed and recalculated.
    /// </param>
    void RebuildCache(DateTime fromTimestamp);

    /// <summary>
    /// Resets the results cache from an index position
    /// and rebuilds it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromIndex">
    /// All periods (inclusive) after this index position will
    /// be removed and recalculated.
    /// </param>
    void RebuildCache(int fromIndex);

    /// <summary>
    /// Add a single new observed item.
    /// We'll determine if it's new or an update.
    /// </summary>
    /// <param name="newIn" cref="ISeries">
    /// Observed item to add or update
    /// </param>
    void Add(TIn newIn);

    /// <summary>
    /// Add a batch of observed items.
    /// We'll determine if they're new or updated.
    /// </summary>
    /// <param name="newIn" cref="ISeries">
    /// Batch of observed items to add or update
    /// </param>
    void Add(IEnumerable<TIn> newIn);

    /// <summary>
    /// Delete an item from the cache.
    /// We'll double-check that it exists in the
    /// cache before propogating the event to subscribers.
    /// </summary>
    /// <remarks>
    /// Developer note: override this method when there
    /// is no provider index parity.
    /// </remarks>
    /// <param name="deletedIn">Source item to match</param>
    /// <param name="index">Provider index position</param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    Act Delete(TIn deletedIn, int? index = null);

    /// <summary>
    /// Returns a short text label for the hub
    /// with parameter values, e.g. "EMA(10)"
    /// </summary>
    /// <returns>String label</returns>
    string ToString();
}
