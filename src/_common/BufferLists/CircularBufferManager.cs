namespace Skender.Stock.Indicators;

/// <summary>
/// A circular buffer implementation that provides efficient fixed-size storage
/// with configurable size optimization and memory management.
/// </summary>
/// <typeparam name="T">The type of data stored in the buffer.</typeparam>
public class CircularBufferManager<T> : IBufferManager<T>
{
    private T[] _buffer;
    private int _head;
    private int _tail;
    private int _count;
    private readonly int _initialCapacity;

    /// <summary>
    /// Initializes a new instance of the <see cref="CircularBufferManager{T}"/> class.
    /// </summary>
    /// <param name="capacity">The initial capacity of the buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when capacity is less than 1.</exception>
    public CircularBufferManager(int capacity)
    {
        if (capacity < 1)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be at least 1.");

        _initialCapacity = capacity;
        _buffer = new T[capacity];
        _head = 0;
        _tail = 0;
        _count = 0;
    }

    /// <inheritdoc/>
    public int Capacity => _buffer.Length;

    /// <inheritdoc/>
    public int Count => _count;

    /// <inheritdoc/>
    public bool IsFull => _count == _buffer.Length;

    /// <inheritdoc/>
    public bool IsEmpty => _count == 0;

    /// <inheritdoc/>
    public void Add(T item)
    {
        _buffer[_tail] = item;
        
        if (IsFull)
        {
            _head = (_head + 1) % _buffer.Length;
        }
        else
        {
            _count++;
        }
        
        _tail = (_tail + 1) % _buffer.Length;
    }

    /// <inheritdoc/>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

            var actualIndex = (_head + index) % _buffer.Length;
            return _buffer[actualIndex];
        }
    }

    /// <inheritdoc/>
    public T GetMostRecent()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Buffer is empty.");

        var lastIndex = _tail == 0 ? _buffer.Length - 1 : _tail - 1;
        return _buffer[lastIndex];
    }

    /// <inheritdoc/>
    public T GetOldest()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Buffer is empty.");

        return _buffer[_head];
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _head = 0;
        _tail = 0;
        _count = 0;
        
        // Clear references for garbage collection
        Array.Clear(_buffer, 0, _buffer.Length);
    }

    /// <inheritdoc/>
    public IReadOnlyList<T> ToReadOnlyList()
    {
        var result = new T[_count];
        
        for (int i = 0; i < _count; i++)
        {
            result[i] = this[i];
        }
        
        return Array.AsReadOnly(result);
    }

    /// <inheritdoc/>
    public void OptimizeSize(int optimalSize)
    {
        if (optimalSize < 1)
            throw new ArgumentOutOfRangeException(nameof(optimalSize), "Optimal size must be at least 1.");

        // Only resize if the optimal size is significantly different
        if (optimalSize == _buffer.Length || 
            (optimalSize < _buffer.Length && optimalSize >= _buffer.Length * 0.75) ||
            (optimalSize > _buffer.Length && optimalSize <= _buffer.Length * 1.25))
        {
            return; // No need to resize
        }

        var newBuffer = new T[optimalSize];
        var itemsToKeep = Math.Min(_count, optimalSize);
        
        // Copy the most recent items to the new buffer
        for (int i = 0; i < itemsToKeep; i++)
        {
            var sourceIndex = itemsToKeep < _count ? _count - itemsToKeep + i : i;
            newBuffer[i] = this[sourceIndex];
        }

        _buffer = newBuffer;
        _head = 0;
        _tail = itemsToKeep % optimalSize;
        _count = itemsToKeep;
    }

    /// <summary>
    /// Gets statistics about buffer usage for monitoring and optimization.
    /// </summary>
    public (double FillRatio, int AccessCount, TimeSpan LastAccessTime) UsageStatistics
    {
        get
        {
            var fillRatio = _count / (double)_buffer.Length;
            
            // These would be tracked in a real implementation
            var accessCount = 0;
            var lastAccessTime = TimeSpan.Zero;
            
            return (fillRatio, accessCount, lastAccessTime);
        }
    }
}