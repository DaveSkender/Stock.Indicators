namespace Skender.Stock.Indicators;

/// <summary>
/// Utility and extension methods for <see cref="BufferList{T}"/> implementations.
/// </summary>
public static class BufferListUtilities
{
    /// <summary>
    /// Updates a rolling buffer by removing the oldest value when at capacity and adding a new value.
    /// This utility maintains a fixed-size FIFO buffer for sliding window calculations.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the buffer.</typeparam>
    /// <param name="buffer">The buffer queue to update.</param>
    /// <param name="capacity">The maximum number of elements allowed in the buffer.</param>
    /// <param name="value">The new value to add to the buffer.</param>
    /// <exception cref="ArgumentNullException">Thrown when buffer is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when capacity is less than or equal to zero.</exception>
    public static void Update<T>(this Queue<T> buffer, int capacity, T value)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 0.");
        }

        if (buffer.Count == capacity)
        {
            buffer.Dequeue();
        }

        buffer.Enqueue(value);
    }

    /// <summary>
    /// Updates a rolling buffer and returns the dequeued value if one was removed.
    /// This is useful for maintaining running sums where you need to know what was removed.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the buffer.</typeparam>
    /// <param name="buffer">The buffer queue to update.</param>
    /// <param name="capacity">The maximum number of elements allowed in the buffer.</param>
    /// <param name="value">The new value to add to the buffer.</param>
    /// <returns>The dequeued value if the buffer was at capacity, otherwise default(T).</returns>
    /// <exception cref="ArgumentNullException">Thrown when buffer is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when capacity is less than or equal to zero.</exception>
    public static T? UpdateWithDequeue<T>(this Queue<T> buffer, int capacity, T value)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 0.");
        }

        T? dequeuedValue = default;

        if (buffer.Count == capacity)
        {
            dequeuedValue = buffer.Dequeue();
        }

        buffer.Enqueue(value);
        return dequeuedValue;
    }
}
