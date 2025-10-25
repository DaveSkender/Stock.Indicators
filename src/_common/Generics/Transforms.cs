using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

/// <summary>
/// Provides extension methods for transforming collections.
/// </summary>
public static class Transforming
{
    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> to a <see cref="Collection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
    /// <param name="source">The source collection to convert.</param>
    /// <returns>A <see cref="Collection{T}"/> containing the elements from the source collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source collection is null.</exception>
    internal static Collection<T> ToCollection<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return [.. source];
    }
}
