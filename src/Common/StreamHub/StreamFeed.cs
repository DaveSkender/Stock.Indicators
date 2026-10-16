using System.Runtime.ExceptionServices;
using System.Threading.Channels;

namespace FacioQuo.Stock.Indicators;

/// <summary>
/// Feeds a root hub from any thread, applying every item with a single writer.
/// </summary>
/// <remarks>
/// Hubs are single-writer, so concurrent producers (a WebSocket or SSE callback,
/// several tasks) write here instead of calling <c>Add</c> themselves.
/// One consumer task drains the queue into the hub in arrival order.
/// While a feed is open, every mutation of the hub goes through it; corrections
/// such as <c>RemoveAt</c> or <c>Rebuild</c> cannot be queued behind its items.
/// Observer callbacks run on the feed's consumer task, not on the writing thread.
/// If the hub rejects an item, the consumer stops and the original exception surfaces on
/// <see cref="Completion"/>, on the next <see cref="WriteAsync(TIn, CancellationToken)"/>,
/// and on <see cref="DisposeAsync"/>.
/// </remarks>
/// <typeparam name="TIn">Type of the items the hub accepts.</typeparam>
public sealed class StreamFeed<TIn> : IAsyncDisposable
{
    private readonly Channel<TIn> _channel;
    private readonly Action? _endTransmission;
    private readonly Action? _closed;
    private volatile ExceptionDispatchInfo? _fault;
    private int _disposed;

    internal StreamFeed(
        Action<TIn> add,
        int capacity,
        StreamFeedFullMode fullMode,
        Action? endTransmission,
        Action? closed = null)
    {
        _endTransmission = endTransmission;
        _closed = closed;

        _channel = Channel.CreateBounded<TIn>(
            new BoundedChannelOptions(capacity) {
                FullMode = fullMode == StreamFeedFullMode.DropOldest
                    ? BoundedChannelFullMode.DropOldest
                    : BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });

        Completion = Task.Run(() => Drain(add));
    }

    /// <summary>
    /// Gets a task that completes when the queue has drained after disposal,
    /// or faults with the exception the hub raised for a rejected item.
    /// </summary>
    public Task Completion { get; }

    /// <summary>
    /// Queues an item for the hub; safe to call from any thread.
    /// </summary>
    /// <param name="item">Item to add to the hub.</param>
    /// <param name="cancellationToken">Cancels the wait for queue space.</param>
    /// <returns>A task that completes when the item is queued, not when the hub has applied it.</returns>
    /// <exception cref="InvalidOperationException">The feed is disposed.</exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    /// <remarks>If the hub rejected an earlier item, this throws that item's original exception.</remarks>
    public async ValueTask WriteAsync(TIn item, CancellationToken cancellationToken = default)
    {
        _fault?.Throw();

        try
        {
            await _channel.Writer.WriteAsync(item, cancellationToken).ConfigureAwait(false);
        }
        catch (ChannelClosedException ex)
        {
            _fault?.Throw();
            throw new InvalidOperationException("The feed is completed.", ex);
        }
    }

    /// <summary>
    /// Stops accepting items, applies those already queued, then optionally ends the transmission.
    /// </summary>
    /// <remarks>
    /// Waiting for the queue to drain cannot be canceled, and takes as long as the hub needs
    /// to apply what is queued.  If the hub rejected an item, this throws its original
    /// exception and does not end the transmission, so subscribers are not completed.
    /// Do not dispose the feed from an observer callback: callbacks run on the consumer
    /// task, which would then wait for itself.
    /// </remarks>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1)
        {
            return;
        }

        try
        {
            _channel.Writer.TryComplete();

            try
            {
                await Completion.ConfigureAwait(false);
            }
#pragma warning disable CA1031, RCS1075 // the consumer's fault is rethrown below with its original stack
            catch (Exception)
            {
                // observed here; _fault carries it
            }
#pragma warning restore CA1031, RCS1075

            _fault?.Throw();
            _endTransmission?.Invoke();
        }
        finally
        {
            _closed?.Invoke();
        }
    }

    // an item that throws stops the loop; later writers see the fault rather than a stalled queue
#pragma warning disable CA1031, RCS1075 // intentional catch-all at the consumer boundary
    private async Task Drain(Action<TIn> add)
    {
        try
        {
            await foreach (TIn item in _channel.Reader.ReadAllAsync().ConfigureAwait(false))
            {
                add(item);
            }
        }
        catch (Exception ex)
        {
            _fault = ExceptionDispatchInfo.Capture(ex);
            _channel.Writer.TryComplete(ex);
            throw;
        }
    }
#pragma warning restore CA1031, RCS1075
}
