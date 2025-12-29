namespace Skender.Stock.Indicators;

/// <summary>
/// Provides O(1) amortized rolling window minimum tracking using a monotonic deque.
/// </summary>
/// <typeparam name="T">The type of values being tracked (must be comparable).</typeparam>
/// <remarks>
/// Uses a deque data structure to maintain values in increasing order, allowing constant-time
/// operations for adding new values and retrieving the minimum. When the window is full,
/// older values are automatically removed. This is significantly more efficient than
/// scanning the entire window on each update (O(n) per operation).
/// </remarks>
internal sealed class RollingWindowMin<T> where T : IComparable<T>
{
    private readonly int _capacity;
    private readonly Queue<T> _window;
    private readonly LinkedList<T> _deque;
    private int _nanCount; // Track NaN values in the window

    /// <summary>
    /// Initializes a new instance of the <see cref="RollingWindowMin{T}"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of elements in the rolling window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when capacity is less than or equal to zero.</exception>
    internal RollingWindowMin(int capacity)
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
        _nanCount = 0;
    }

    /// <summary>
    /// Gets the current minimum value in the rolling window.
    /// </summary>
    /// <returns>
    /// The minimum value. Returns NaN (for numeric types) if any NaN values are present in the window.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the window is empty.
    /// </exception>
    internal T GetMin()
    {
        if (_window.Count == 0)
        {
            throw new InvalidOperationException("Cannot retrieve minimum from an empty rolling window.");
        }

        // If NaN is present in the window, return NaN for numeric types
        if (_nanCount > 0)
        {
            if (typeof(T) == typeof(double))
            {
                return (T)(object)double.NaN;
            }

            if (typeof(T) == typeof(float))
            {
                return (T)(object)float.NaN;
            }
        }

        if (_deque.Count == 0)
        {
            throw new InvalidOperationException("Cannot retrieve minimum from an empty rolling window.");
        }

        return _deque.First!.Value;
    }

    /// <summary>
    /// Gets the current number of elements in the rolling window.
    /// </summary>
    internal int Count => _window.Count;

    /// <summary>
    /// Adds a new value to the rolling window.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <remarks>
    /// If the window is full, the oldest value is automatically removed.
    /// The operation maintains the monotonic increasing property of the deque,
    /// ensuring O(1) amortized time complexity.
    /// NaN values are accepted and will cause Min to return NaN when present in the window.
    /// </remarks>
    internal void Add(T value)
    {
        // Check for NaN - if present, track it
        bool isNaN = IsNaN(value);
        if (isNaN)
        {
            _nanCount++;
        }

        // Add to window
        _window.Enqueue(value);

        if (!isNaN)
        {
            // Remove elements from back of deque that are larger than the new value
            while (_deque.Count > 0 && _deque.Last!.Value.CompareTo(value) > 0)
            {
                _deque.RemoveLast();
            }

            // Add new value to back of deque
            _deque.AddLast(value);
        }

        // Remove oldest element if window exceeds capacity
        if (_window.Count > _capacity)
        {
            T oldValue = _window.Dequeue();
            bool oldIsNaN = IsNaN(oldValue);

            if (oldIsNaN)
            {
                _nanCount--;
            }

            // Remove from front of deque if it was the oldest minimum
            if (!oldIsNaN && _deque.Count > 0 && _deque.First!.Value.CompareTo(oldValue) == 0)
            {
                _deque.RemoveFirst();
            }
        }
    }

    /// <summary>
    /// Resets the rolling window, removing all elements.
    /// </summary>
    internal void Clear()
    {
        _window.Clear();
        _deque.Clear();
        _nanCount = 0;
    }

    private static bool IsNaN(T value)
        => (value is double d && double.IsNaN(d))
        || (value is float f && float.IsNaN(f));
}
