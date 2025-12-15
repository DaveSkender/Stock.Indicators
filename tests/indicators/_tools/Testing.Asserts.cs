namespace Tests.Tools;

public static class TestAsserts
{
    /// <summary>
    /// Asserts that all computed (non-null, non-NaN) values remain within inclusive bounds.
    /// </summary>
    /// <typeparam name="T">Result type containing the bounded value.</typeparam>
    /// <param name="results">Result collection to evaluate.</param>
    /// <param name="selector">Selector that returns the bounded value to check.</param>
    /// <param name="minInclusive">Minimum inclusive bound.</param>
    /// <param name="maxInclusive">Maximum inclusive bound.</param>
    public static void AlwaysBounded<T>(
        IEnumerable<T> results,
        Func<T, double?> selector,
        double minInclusive,
        double maxInclusive)
    {
        ArgumentNullException.ThrowIfNull(results);
        ArgumentNullException.ThrowIfNull(selector);

        foreach (T result in results)
        {
            double? value = selector(result);

            if (!value.HasValue || double.IsNaN(value.Value))
            {
                continue;
            }

            value.Value.Should().BeGreaterThanOrEqualTo(minInclusive);
            value.Value.Should().BeLessThanOrEqualTo(maxInclusive);
        }
    }
}
