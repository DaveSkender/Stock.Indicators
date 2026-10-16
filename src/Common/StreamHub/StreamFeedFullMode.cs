namespace FacioQuo.Stock.Indicators;

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
