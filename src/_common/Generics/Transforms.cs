using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// GENERIC TRANSFORMS

public static class Transforms
{
    // TO COLLECTION
    internal static Collection<T> ToCollection<T>(this IEnumerable<T> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        Collection<T> collection = new();

        foreach (T item in source)
        {
            collection.Add(item);
        }

        return collection;
    }
}
