using System.Collections;

namespace Skender.Stock.Indicators;

/// <summary>
/// Abstract base class for buffered indicator lists.
/// </summary>
/// <typeparam name="TResult">The type of results stored in the list.</typeparam>
/// <remarks>
/// This base class provides a simplified collection interface that only exposes
/// safe operations for buffer list implementations. It does not support removal
/// or modification operations that would desynchronize internal buffer state.
/// </remarks>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Naming",
    "CA1710:Identifiers should have correct suffix",
    Justification = "BufferList is the established naming convention for this library")]
public abstract class BufferList<TResult> : ICollection<TResult>, IReadOnlyList<TResult>
    where TResult : ISeries
{
    private const int DefaultMaxListSize = (int)(int.MaxValue * 0.9);
    private int _maxListSize = DefaultMaxListSize;
    private readonly List<TResult> _internalList = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferList{TResult}"/> class.
    /// </summary>
    protected BufferList()
    {
    }

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
        set
        {
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
    /// Clears the internal list storage.
    /// </summary>
    protected void ClearInternal()
    {
        _internalList.Clear();
    }

    /// <summary>
    /// Removes the item at the specified index from the internal list.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    protected void RemoveAtInternal(int index)
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

    #region ICollection<TResult> Implementation

    /// <inheritdoc/>
    void ICollection<TResult>.Add(TResult item)
    {
        throw new NotSupportedException(
            "Direct use of Add(TResult) is not supported. " +
            "Use the strongly-typed Add methods specific to this indicator.");
    }

    /// <inheritdoc/>
    public abstract void Clear();

    /// <inheritdoc/>
    public bool Contains(TResult item) => _internalList.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(TResult[] array, int arrayIndex)
        => _internalList.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<TResult> GetEnumerator() => _internalList.GetEnumerator();

    /// <inheritdoc/>
    bool ICollection<TResult>.Remove(TResult item)
    {
        throw new NotSupportedException(
            "Remove is not supported. Buffer lists do not support item removal.");
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}
