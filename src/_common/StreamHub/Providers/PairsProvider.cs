namespace Skender.Stock.Indicators;

/// <summary>
/// Base provider for dual-stream indicators that require synchronized pairs of reusable inputs.
/// </summary>
/// <typeparam name="TIn">The type of input data (must be IReusable).</typeparam>
/// <typeparam name="TOut">The type of output data (must be IReusable).</typeparam>
/// <param name="providerA">First streaming data provider.</param>
/// <param name="providerB">Second streaming data provider (must be synchronized with providerA).</param>
/// <remarks>
/// This base class encapsulates common dual-stream patterns including:
/// - Dual-cache management
/// - Built-in timestamp synchronization validation
/// - Synchronized input requirements
/// </remarks>
public abstract class PairsProvider<TIn, TOut>(
    IStreamObservable<TIn> providerA,
    IStreamObservable<TIn> providerB
) : StreamHub<TIn, TOut>(providerA), IPairsProvider<TOut>
    where TIn : IReusable
    where TOut : IReusable
{
    /// <summary>
    /// Gets the reference to the second provider's cache.
    /// </summary>
    protected IReadOnlyList<TIn> ProviderCacheB { get; } =
        (providerB ?? throw new ArgumentNullException(nameof(providerB))).GetCacheRef();

    /// <summary>
    /// Gets a readonly reference to the second provider's input cache.
    /// </summary>
    /// <returns>Read-only list of cached input items from the second series.</returns>
    public IReadOnlyList<IReusable> GetCacheBRef()
    {
        // Use a safe adapter that works with both reference and value types
        return new ReusableListAdapter<TIn>(ProviderCacheB);
    }

    /// <summary>
    /// Validates that timestamps match between both provider caches at the specified index.
    /// </summary>
    /// <param name="index">The index to validate.</param>
    /// <param name="currentItem">The current item being processed.</param>
    /// <exception cref="InvalidQuotesException">Thrown when timestamps don't match.</exception>
    protected void ValidateTimestampSync(int index, TIn currentItem)
    {
        if (index < ProviderCache.Count && index < ProviderCacheB.Count)
        {
            if (ProviderCache[index].Timestamp != ProviderCacheB[index].Timestamp)
            {
                throw new InvalidQuotesException(
                    nameof(currentItem), currentItem.Timestamp,
                    "Timestamp sequence does not match. " +
                    "Dual-stream indicators require matching dates in provided histories.");
            }
        }
    }

    /// <summary>
    /// Checks if both caches have sufficient data at the specified index.
    /// </summary>
    /// <param name="index">The index to check.</param>
    /// <param name="minimumPeriods">The minimum number of periods required.</param>
    /// <returns>True if both caches have sufficient data.</returns>
    protected bool HasSufficientData(int index, int minimumPeriods)
        => index >= minimumPeriods - 1
           && ProviderCacheB.Count > index;
}
