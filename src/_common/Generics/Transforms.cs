using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// GENERIC TRANSFORMS

public static class Transforming
{
    // TO COLLECTION
    internal static Collection<T> ToCollection<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        Collection<T> collection = [.. source];

        return collection;
    }
}
