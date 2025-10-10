using System.Collections;

namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for <see cref="BufferList{TResult}"/>,
/// based on <see cref="IReadOnlyList{TResult}"/>.
/// </summary>
public interface IBufferList<TResult> : IReadOnlyList<TResult>
{
    /// <summary>
    /// Clears the internal <see cref="ICollection{TResult}" /> storage.
    /// </summary>
    void Clear();

    /// <inheritdoc/>
    bool Contains(TResult item);

    /// <inheritdoc/>
    void CopyTo(TResult[] array, int arrayIndex);

    /* IReadOnlyList<T> */
    //TResult IReadOnlyList<TResult>.this[int index] => throw new NotImplementedException();
    //int IReadOnlyCollection<TResult>.Count => throw new NotImplementedException();
    //IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => throw new NotImplementedException();
    //IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    /* NOTE: ICollection<T> contains members that are problematic
     *       for our use case, and are include here for reference.
     * 
     * bool IsReadOnly;                     // implicit in use of IReadOnlyList<T>
     * int Count;                           // redundant to IReadOnlyList<T> property
     * IEnumerator<TResult> GetEnumerator() // redundant to IReadOnlyList<T> member
     * void Add(TResult item);              // bypasses primary use case
     * bool Remove(TResult item);           // corrupts timeline and chaining
     */
}
