using System.Collections;

namespace Skender.Stock.Indicators;

/// <summary>
/// Abstract base class for buffered indicator lists that provides
/// state tracking and rollback for manual list modifications.
/// </summary>
/// <typeparam name="TResult">The type of results stored in the list.</typeparam>
/// <remarks>
/// <para>
/// This base class detects when the list has been manually modified
/// (e.g., via Remove, RemoveAt, RemoveRange, or indexer assignment)
/// and automatically rolls back internal buffer state to maintain consistency.
/// </para>
/// <para>
/// When a modification is detected, the <see cref="RollbackState"/> method
/// is called to reset internal buffers to match the current list state.
/// </para>
/// </remarks>
public abstract class BufferListBase<TResult> : IList<TResult>
    where TResult : ISeries
{
    private readonly List<TResult> _internalList = [];
    private int _expectedCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferListBase{TResult}"/> class.
    /// </summary>
    protected BufferListBase()
    {
        _expectedCount = 0;
    }

    /// <inheritdoc/>
    public TResult this[int index]
    {
        get => _internalList[index];
        set {
            _internalList[index] = value;
            // Indexer assignment counts as a modification
            ValidateAndRollback();
        }
    }

    /// <inheritdoc/>
    public int Count => _internalList.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// Adds an item to the list using internal buffer logic.
    /// </summary>
    /// <param name="item">The item to add.</param>
    protected void AddInternal(TResult item)
    {
        _internalList.Add(item);
        _expectedCount = _internalList.Count;
    }

    /// <summary>
    /// Clears the internal list storage and resets tracking.
    /// </summary>
    protected void ClearInternal()
    {
        _internalList.Clear();
        _expectedCount = 0;
    }

    /// <summary>
    /// Validates that the list count matches expected count and rolls back if needed.
    /// </summary>
    private void ValidateAndRollback()
    {
        if (_internalList.Count != _expectedCount)
        {
            // List was manually modified - rollback state to match
            RollbackState(_internalList.Count > 0 ? _internalList.Count - 1 : -1);
            _expectedCount = _internalList.Count;
        }
    }

    /// <summary>
    /// Rollbacks internal buffer state to match the list at a specific index position.
    /// </summary>
    /// <param name="index">
    /// The index position to rollback to. Use -1 to reset to initial state (empty).
    /// </param>
    /// <remarks>
    /// Derived classes must implement this method to reset their internal buffers,
    /// running sums, EMA values, and other stateful data to match the state that
    /// would exist if only the elements from index 0 to <paramref name="index"/>
    /// had been added.
    /// </remarks>
    protected abstract void RollbackState(int index);

    #region IList<TResult> Implementation

    /// <inheritdoc/>
    public void Add(TResult item)
    {
        throw new NotSupportedException(
            "Direct use of Add(TResult) is not supported. " +
            "Use the strongly-typed Add methods specific to this indicator.");
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ClearInternal();
        RollbackState(-1);
    }

    /// <inheritdoc/>
    public bool Contains(TResult item) => _internalList.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(TResult[] array, int arrayIndex)
        => _internalList.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<TResult> GetEnumerator() => _internalList.GetEnumerator();

    /// <inheritdoc/>
    public int IndexOf(TResult item) => _internalList.IndexOf(item);

    /// <inheritdoc/>
    public void Insert(int index, TResult item)
    {
        throw new NotSupportedException(
            "Insert is not supported. Buffer lists only support sequential addition via Add methods.");
    }

    /// <inheritdoc/>
    public bool Remove(TResult item)
    {
        bool removed = _internalList.Remove(item);
        if (removed)
        {
            ValidateAndRollback();
        }

        return removed;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        _internalList.RemoveAt(index);
        ValidateAndRollback();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}
