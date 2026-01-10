namespace Skender.Stock.Indicators;

/// <summary>
/// Base interface for stream hubs that expose cached indicator results.
/// Provides a covariant (<c>out</c>) result type for use by <see cref="IStreamHub{TOut}"/> implementations.
/// </summary>
/// <typeparam name="TOut">
/// Reusable result type returned by the stream hub and stored in the <see cref="Results"/> cache.
/// </typeparam>
public interface IStreamHubBase<out TOut>
    where TOut : IReusable
{
    /// <summary>
    /// Read-only list of the stored cache values.
    /// </summary>
    IReadOnlyList<TOut> Results { get; }
}
