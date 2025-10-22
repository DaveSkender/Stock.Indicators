namespace Skender.Stock.Indicators;

/// <summary>
/// Provides O(1) amortized rolling window maximum tracking using a monotonic deque.
/// </summary>
/// <typeparam name="T">The type of values being tracked (must be comparable).</typeparam>
/// <remarks>
/// Uses a deque data structure to maintain values in decreasing order, allowing constant-time
/// operations for adding new values and retrieving the maximum. When the window is full,
/// older values are automatically removed. This is significantly more efficient than
/// scanning the entire window on each update (O(n) per operation).
/// </remarks>
public sealed class RollingWindowMax<T> where T : IComparable<T>
{
    private readonly int _capacity;
    private readonly Queue<T> _window;
    private readonly LinkedList<T> _deque;

    /// <summary>
    /// Initializes a new instance of the <see cref="RollingWindowMax{T}"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of elements in the rolling window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when capacity is less than or equal to zero.</exception>
    public RollingWindowMax(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                "Capacity must be greater than zero.");
        }

        _capacity = capacity;
        _window = new Queue<T>(capacity);
        _deque = new LinkedList<T>();
    }

    /// <summary>
    /// Gets the current maximum value in the rolling window.
    /// </summary>
    /// <value>
    /// The maximum value, or default(T) if the window is empty.
    /// </value>
    public T Max => _deque.Count == 0
        ? throw new InvalidOperationException("Cannot retrieve maximum from an empty rolling window.")
        : _deque.First!.Value;

    /// <summary>
    /// Gets the current number of elements in the rolling window.
    /// </summary>
    public int Count => _window.Count;

    /// <summary>
    /// Adds a new value to the rolling window.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <remarks>
    /// If the window is full, the oldest value is automatically removed.
    /// The operation maintains the monotonic decreasing property of the deque,
    /// ensuring O(1) amortized time complexity.
    /// </remarks>
    public void Add(T value)
    {
        // Reject NaN values to keep the deque invariant.
        ValidateFiniteValue(value);

        // Add to window
        _window.Enqueue(value);

        // Remove elements from back of deque that are smaller than the new value
        while (_deque.Count > 0 && _deque.Last!.Value.CompareTo(value) < 0)
        {
            _deque.RemoveLast();
        }

        // Add new value to back of deque
        _deque.AddLast(value);

        // Remove oldest element if window exceeds capacity
        if (_window.Count > _capacity)
        {
            T oldValue = _window.Dequeue();

            // Remove from front of deque if it was the oldest maximum
            if (_deque.Count > 0 && _deque.First!.Value.CompareTo(oldValue) == 0)
            {
                _deque.RemoveFirst();
            }
        }
    }

    /// <summary>
    /// Resets the rolling window, removing all elements.
    /// </summary>
    public void Clear()
    {
        _window.Clear();
        _deque.Clear();
    }

    private static void ValidateFiniteValue(T value)
    {
        if (value is double d && double.IsNaN(d))
        {
            throw new ArgumentException("Rolling window cannot accept NaN values.", nameof(value));
        }

        if (value is float f && float.IsNaN(f))
        {
            throw new ArgumentException("Rolling window cannot accept NaN values.", nameof(value));
        }
    }
}
