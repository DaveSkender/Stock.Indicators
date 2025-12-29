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
public abstract class BufferList<TResult> : IReadOnlyList<TResult>
    where TResult : ISeries
{
    private readonly List<TResult> _internalList = [];

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
    /// Gets the name of the buffer list.
    /// </summary>
    public string Name { get; protected init; } = null!;

    /// <summary>
    /// Gets or sets the maximum number of results to retain in the list.
    /// When the list exceeds this value, the oldest items are pruned.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a parameter is out of the valid range</exception>
    public int MaxListSize
    {
        get;
        set {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "MaxListSize must be greater than 0.");
            }

            field = value;

            if (_internalList.Count > field)
            {
                PruneList();
            }
        }
    } = (int)(int.MaxValue * 0.9);

    /// <summary>
    /// Adds an item to the list using internal buffer logic.
    /// Automatically prunes the list if it exceeds MaxListSize.
    /// </summary>
    /// <param name="item">The item to add.</param>
    protected void AddInternal(TResult item)
    {
        _internalList.Add(item);

        if (_internalList.Count > MaxListSize)
        {
            PruneList();
        }
    }

    /// <summary>
    /// Updates an existing item in the list at the specified index.
    /// Used for indicators that need to revise historical values (repaint).
    /// </summary>
    /// <param name="index">The zero-based index of the item to update.</param>
    /// <param name="item">The updated item.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when index is out of range.</exception>
    protected void UpdateInternal(int index, TResult item)
    {
        if (index < 0 || index >= _internalList.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index),
                "Index must be within the bounds of the list.");
        }

        _internalList[index] = item;
    }

    /// <summary>
    /// Removes the item at the specified index from the internal list.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    protected void RemoveAt(int index) => _internalList.RemoveAt(index);

    /// <summary>
    /// Removes the oldest results when the list exceeds <see cref="MaxListSize"/>.
    /// Can be overridden in derived classes for custom pruning logic.
    /// </summary>
    protected virtual void PruneList()
    {
        int overflow = _internalList.Count - MaxListSize;

        if (overflow <= 0)
        {
            return;
        }

        _internalList.RemoveRange(0, overflow);
    }

    /* IList<TResult> features (limited set) */

    /// <inheritdoc cref="List{TResult}.Clear"/>
    public virtual void Clear() => _internalList.Clear();

    /// <inheritdoc cref="List{TResult}.Contains(TResult)"/>
    public bool Contains(TResult item) => _internalList.Contains(item);

    /// <inheritdoc cref="List{TResult}.CopyTo(TResult[], int)"/>
    public void CopyTo(TResult[] array, int arrayIndex)
        => _internalList.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<TResult> GetEnumerator() => _internalList.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => Name;
}
