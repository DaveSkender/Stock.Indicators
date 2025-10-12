using System.Collections;

namespace Skender.Stock.Indicators;

/// <summary>
/// Lightweight adapter that safely exposes IReadOnlyList&lt;TIn&gt; as IReadOnlyList&lt;IReusable&gt;
/// by forwarding operations and boxing value types when necessary.
/// </summary>
/// <typeparam name="TIn">The underlying type (must implement IReusable).</typeparam>
/// <remarks>
/// This adapter avoids InvalidCastException when TIn is a value type (e.g., record struct)
/// because covariance doesn't work with value types. The adapter boxes each element on access
/// but does not modify or copy the underlying collection.
/// </remarks>
internal sealed class ReusableListAdapter<TIn>(
    IReadOnlyList<TIn> source
) : IReadOnlyList<IReusable>
    where TIn : IReusable
{
    private readonly IReadOnlyList<TIn> _source = source ?? throw new ArgumentNullException(nameof(source));

    public int Count => _source.Count;

    public IReusable this[int index] => _source[index];

    public IEnumerator<IReusable> GetEnumerator()
    {
        foreach (TIn item in _source)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
