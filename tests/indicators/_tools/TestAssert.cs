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

    /// <summary>
    /// Asserts that two <see cref="IEnumerable{T}" /> series are equivalent with floating-point precision tolerance.
    /// Use this for indicators with recursive calculations that accumulate floating-point precision differences.
    /// </summary>
    /// <typeparam name="T">List elements must be <see cref="ISeries"/> interface types.</typeparam>
    /// <param name="actual">The actual list of series to be compared.</param>
    /// <param name="expected">The expected list of series to compare against.</param>
    /// <param name="precision">Decimal precision for double comparisons (default: 13 decimal places).</param>
    public static void IsApproximately<T>(
        this IEnumerable<T> actual,
        IEnumerable<T> expected,
        int precision = 14
    ) where T : ISeries

        // TODO: Revisit all places using this or similar precision-based comparison to ensure precision
        // is appropriate.  Ideally, we want to remove this method and have algebraically stable algorithms.

        => actual.Should().BeEquivalentTo(expected, options => options
            // Require same sequence order; prevents passing when items are equivalent but re-ordered
            .WithStrictOrdering()
            // Compare instances of T by their members (property values) rather than reference or Equals
            .ComparingByMembers<T>()
            // Use approximate comparison for double/double? properties with specified precision
            .Using<double>(ctx => {
                // Handle NaN explicitly - both must be NaN or neither
                if (double.IsNaN(ctx.Expectation) && double.IsNaN(ctx.Subject))
                {
                    // Both are NaN, match successful
                }
                else if (double.IsNaN(ctx.Expectation) || double.IsNaN(ctx.Subject))
                {
                    ctx.Subject.Should().Be(ctx.Expectation); // Will fail with clear message
                }
                else
                {
                    ctx.Subject.Should().BeApproximately(ctx.Expectation, Math.Pow(10, -precision));
                }
            }).WhenTypeIs<double>()
            .Using<double?>(ctx => {
                if (ctx.Expectation.HasValue && ctx.Subject.HasValue)
                {
                    double expected = ctx.Expectation.Value;
                    double actual = ctx.Subject.Value;

                    // Handle NaN explicitly - both must be NaN or neither
                    if (double.IsNaN(expected) && double.IsNaN(actual))
                    {
                        // Both are NaN, match successful
                    }
                    else if (double.IsNaN(expected) || double.IsNaN(actual))
                    {
                        ctx.Subject.Should().Be(ctx.Expectation); // Will fail with clear message
                    }
                    else
                    {
                        actual.Should().BeApproximately(expected, Math.Pow(10, -precision));
                    }
                }
                else
                {
                    ctx.Subject.Should().Be(ctx.Expectation);
                }
            }).WhenTypeIs<double?>());
}
