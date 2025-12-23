namespace Test.Tools;

/// <summary>
/// Custom assertion helpers for test validation.
/// </summary>
public static class TestAssert
{
    /// <summary>
    /// Asserts that all computed (non-null, non-NaN) values remain within inclusive bounds.
    /// </summary>
    /// <typeparam name="T">Result type containing the bounded value.</typeparam>
    /// <param name="results">Result collection to evaluate.</param>
    /// <param name="selector">Selector that returns the bounded value to check.</param>
    /// <param name="minInclusive">Minimum inclusive bound.</param>
    /// <param name="maxInclusive">Maximum inclusive bound.</param>
    public static void IsBetween<T>(
        this IEnumerable<T> results,
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

    /// <summary>
    /// Asserts that two <see cref="IEnumerable{T}" /> series are equivalent with a precision profile.
    /// </summary>
    /// <typeparam name="T">List elements must be <see cref="ISeries"/> interface types.</typeparam>
    /// <param name="actual">The actual list of series to be compared.</param>
    /// <param name="expected">The expected list of series to compare against.</param>
    public static void IsExactly<T>(
        this IEnumerable<T> actual,
        IEnumerable<T> expected
    ) where T : ISeries
        => actual.Should().BeEquivalentTo(expected, static options => options
            // Require same sequence order; prevents passing when items are equivalent but re-ordered
            .WithStrictOrdering()
            // Compare instances of T by their members (property values) rather than reference or Equals
            // This enforces value-based structural equivalency for our result records/classes
            .ComparingByMembers<T>());
}
