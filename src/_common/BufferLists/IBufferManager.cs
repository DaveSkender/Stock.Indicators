namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for circular buffer operations and memory-efficient buffer lifecycle management.
/// </summary>
/// <typeparam name="T">The type of data stored in the buffer.</typeparam>
public interface IBufferManager<T>
{
    /// <summary>
    /// Gets the maximum capacity of the buffer.
    /// </summary>
    int Capacity { get; }

    /// <summary>
    /// Gets the current number of items in the buffer.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets a value indicating whether the buffer is full.
    /// </summary>
    bool IsFull { get; }

    /// <summary>
    /// Gets a value indicating whether the buffer is empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Adds an item to the buffer. If the buffer is full, the oldest item is overwritten.
    /// </summary>
    /// <param name="item">The item to add.</param>
    void Add(T item);

    /// <summary>
    /// Gets the item at the specified index, where 0 is the oldest item.
    /// </summary>
    /// <param name="index">The zero-based index of the item to get.</param>
    /// <returns>The item at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when index is out of range.</exception>
    T this[int index] { get; }

    /// <summary>
    /// Gets the most recently added item.
    /// </summary>
    /// <returns>The most recent item.</returns>
    /// <exception cref="InvalidOperationException">Thrown when buffer is empty.</exception>
    T GetMostRecent();

    /// <summary>
    /// Gets the oldest item in the buffer.
    /// </summary>
    /// <returns>The oldest item.</returns>
    /// <exception cref="InvalidOperationException">Thrown when buffer is empty.</exception>
    T GetOldest();

    /// <summary>
    /// Clears all items from the buffer.
    /// </summary>
    void Clear();

    /// <summary>
    /// Gets all items in the buffer as a read-only list, ordered from oldest to newest.
    /// </summary>
    /// <returns>A read-only list containing all buffer items.</returns>
    IReadOnlyList<T> ToReadOnlyList();

    /// <summary>
    /// Optimizes buffer size based on usage patterns.
    /// </summary>
    /// <param name="optimalSize">The optimal buffer size based on current usage.</param>
    void OptimizeSize(int optimalSize);
}