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
/// If the hub rejects an item, the failure surfaces on the next
/// <see cref="WriteAsync(TIn, CancellationToken)"/> and on <see cref="DisposeAsync"/>.
/// </remarks>
/// <typeparam name="TIn">Type of the items the hub accepts.</typeparam>
public sealed class StreamFeed<TIn> : IAsyncDisposable
{
    private readonly Channel<TIn> _channel;
    private readonly Task _consumer;
    private readonly Action? _endTransmission;
    private volatile ExceptionDispatchInfo? _fault;
    private int _disposed;

    internal StreamFeed(
        Action<TIn> add,
        int capacity,
        StreamFeedFullMode fullMode,
        Action? endTransmission)
    {
        _endTransmission = endTransmission;

        _channel = Channel.CreateBounded<TIn>(
            new BoundedChannelOptions(capacity) {
                FullMode = fullMode == StreamFeedFullMode.DropOldest
                    ? BoundedChannelFullMode.DropOldest
                    : BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });

        _consumer = Task.Run(() => Drain(add));
    }

    /// <summary>
    /// Queues an item for the hub; safe to call from any thread.
    /// </summary>
    /// <param name="item">Item to add to the hub.</param>
    /// <param name="cancellationToken">Cancels the wait for queue space.</param>
    /// <returns>A task that completes when the item is queued, not when the hub has applied it.</returns>
    /// <exception cref="InvalidOperationException">
    /// The feed is disposed, or the hub rejected an earlier item.
    /// </exception>
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
    /// <exception cref="InvalidOperationException">The hub rejected an item.</exception>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1)
        {
            return;
        }

        _channel.Writer.TryComplete();
        await _consumer.ConfigureAwait(false);

        _fault?.Throw();
        _endTransmission?.Invoke();
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
        }
    }
#pragma warning restore CA1031, RCS1075
}

/// <summary>
/// What a <see cref="StreamFeed{TIn}"/> does with a new item when its queue is full.
/// </summary>
public enum StreamFeedFullMode
{
    /// <summary>
    /// The writer waits for space, so no item is lost.
    /// </summary>
    Wait = 0,

    /// <summary>
    /// The oldest queued item is discarded to make room; items are lost under load.
    /// </summary>
    DropOldest = 1
}
