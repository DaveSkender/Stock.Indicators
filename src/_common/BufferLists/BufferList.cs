using System.Collections;

namespace Skender.Stock.Indicators;

/// <summary>
/// Standalone indicator lists with buffering cache.
/// </summary>
/// <remarks>
/// Based on <see cref="IReadOnlyList{TResult}"/>, this class
/// contains core buffering list features for safe operations.
/// </remarks>
/// <inheritdoc/>
public abstract class BufferList<TResult> : IBufferList<TResult>
    where TResult : ISeries
{
    private readonly List<TResult> _internalList = [];

    private const int DefaultMaxListSize = (int)(int.MaxValue * 0.9);
    private int _maxListSize = DefaultMaxListSize;

    /// <summary>
    /// Gets the result at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the result to get.</param>
    /// <returns>The result at the specified index.</returns>
    public TResult this[int index] => _internalList[index];

    /// <inheritdoc/>
    public int Count => _internalList.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <summary>
    /// Gets or sets the maximum number of results to retain in the list.
    /// When the list exceeds this value, the oldest items are pruned.
    /// </summary>
    public int MaxListSize
    {
        get => _maxListSize;
        set {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "MaxListSize must be greater than 0.");
            }

            _maxListSize = value;

            if (_internalList.Count > _maxListSize)
            {
                PruneList();
            }
        }
    }

    /// <summary>
    /// Adds an item to the list using internal buffer logic.
    /// Automatically prunes the list if it exceeds MaxListSize.
    /// </summary>
    /// <param name="item">The item to add.</param>
    protected void AddInternal(TResult item)
    {
        _internalList.Add(item);

        if (_internalList.Count > _maxListSize)
        {
            PruneList();
        }
    }

    /// <summary>
    /// Removes the item at the specified index from the internal list.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    protected void RemoveAt(int index)
    {
        _internalList.RemoveAt(index);
    }

    /// <summary>
    /// Removes the oldest results when the list exceeds <see cref="MaxListSize"/>.
    /// Can be overridden in derived classes for custom pruning logic.
    /// </summary>
    protected virtual void PruneList()
    {
        int overflow = _internalList.Count - _maxListSize;

        if (overflow <= 0)
        {
            return;
        }

        _internalList.RemoveRange(0, overflow);
    }

    /* IList<TResult> features (limited set) */

    /// <inheritdoc cref="List{TResult}.Clear"/>
    public virtual void Clear()
    {
        _internalList.Clear();
    }

    /// <inheritdoc cref="List{TResult}.Contains(TResult)"/>
    public bool Contains(TResult item) => _internalList.Contains(item);

    /// <inheritdoc cref="List{TResult}.CopyTo(TResult[], int)"/>
    public void CopyTo(TResult[] array, int arrayIndex)
        => _internalList.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<TResult> GetEnumerator() => _internalList.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
