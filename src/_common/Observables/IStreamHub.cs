namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub that manages observers, observables, and its cache.
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public interface IStreamHub<TIn, TOut>
    where TIn : struct, ISeries
    where TOut : struct, ISeries
{
    /// <summary>
    /// Returns a short text label
    /// with parameter values, e.g. "EMA(10)"
    /// </summary>
    /// <returns>String label</returns>
    string ToString();

    /// <summary>
    /// Read-only cache of processed results
    /// </summary>
    IReadOnlyList<TOut> Results { get; }

    /// <summary>
    /// Handles new data from provider
    /// </summary>
    /// <param name="act" cref="Act">Caching instruction</param>
    /// <param name="inbound">New value from provider</param>
    void OnNextArrival(Act act, TIn inbound);

    /// <inheritdoc cref="IStreamObserver{TIn}.Unsubscribe" />
    void Unsubscribe();
}

/// <summary>
/// Provider-only hub that manages observables and its cache
/// </summary>
/// <typeparam name="TOut"></typeparam>
public interface IProviderHub<TOut>
    where TOut : struct, ISeries
{
    /// <summary>
    /// Returns a short text label
    /// with parameter values, e.g. "500 Quotes"
    /// </summary>
    /// <returns>String label</returns>
    string ToString();
}

