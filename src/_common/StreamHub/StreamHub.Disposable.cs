namespace Skender.Stock.Indicators;

// STREAM HUB (DISPOSABLE / TEARDOWN)

public abstract partial class StreamHub<TIn, TOut> : IDisposable, IChainDisposable
{
    private bool _disposed;

    /// <summary>
    /// Tears down this hub: stops observing its provider and completes its own
    /// direct subscribers (each receives <see cref="IStreamObserver{T}.OnCompleted"/>
    /// and unsubscribes). Idempotent.
    /// </summary>
    /// <remarks>
    /// This is a <em>single-hop</em> teardown: it completes this hub's direct
    /// subscribers but does not recurse — a subscriber that is itself a hub is
    /// detached from this hub but keeps its own subscribers. To tear down a
    /// whole chain, call <see cref="DisposeChain"/> on the root.
    /// <para>
    /// Like the other mutating operations, call this from the single writer
    /// that feeds the hub; disposing while a concurrent <c>Add</c> is in flight
    /// is undefined.
    /// </para>
    /// </remarks>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the resources used by this hub.
    /// </summary>
    /// <param name="disposing">
    /// True when called from <see cref="Dispose()"/>; false from a finalizer.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            // detach from the provider (stop receiving),
            Unsubscribe();

            // then complete and release downstream subscribers.
            EndTransmission();
        }

        _disposed = true;
    }

    /// <summary>
    /// Tears down this hub and every hub downstream of it, depth-first. Each
    /// hub in the chain has <see cref="Dispose()"/> called on it; any non-hub
    /// observer is completed when the hub it subscribes to is disposed (via
    /// that hub's <see cref="IStreamObservable{T}.EndTransmission"/>). Call on
    /// the root hub to dispose the whole chain in one step. Idempotent.
    /// </summary>
    /// <remarks>
    /// Call from the single writer that feeds the chain; tearing down while a
    /// concurrent <c>Add</c> is in flight is undefined.
    /// </remarks>
    public void DisposeChain()
    {
        // snapshot first: disposing a downstream hub unsubscribes it, which
        // mutates this hub's observer set during the walk
        foreach (IStreamObserver<TOut> observer in _observers.ToArray())
        {
            if (observer is IChainDisposable downstream)
            {
                downstream.DisposeChain();
            }
        }

        Dispose();
    }
}

/// <summary>
/// Internal, type-erased view of a hub's chain-teardown capability. Lets a hub
/// recurse into its downstream observers (typed only as
/// <see cref="IStreamObserver{T}"/>) without knowing their output type.
/// </summary>
internal interface IChainDisposable : IDisposable
{
    /// <summary>
    /// Tears down this hub and everything downstream of it.
    /// </summary>
    void DisposeChain();
}
