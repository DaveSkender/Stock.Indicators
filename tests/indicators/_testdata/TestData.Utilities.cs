using FluentAssertions.Equivalency;

namespace Tests.Data;

internal static partial class Utilities
{
    /// <inheritdoc cref="AssertEquals{T}(IEnumerable{T}, IEnumerable{T}, Precision)" />
    internal static void AssertEquals<T>(
        this IEnumerable<T> actual,
        IEnumerable<T> expected
    ) where T : ISeries
    => actual.Should().BeEquivalentTo(expected, options => options
        // Require same sequence order; prevents passing when items are equivalent but re-ordered
        .WithStrictOrdering()
        // Compare instances of T by their members (property values) rather than reference or Equals
        // This enforces value-based structural equivalency for our result records/classes
        .ComparingByMembers<T>());

    /// <summary>
    /// Asserts that two <see cref="IEnumerable{T}" /> series are equivalent.
    /// </summary>
    /// <typeparam name="T">
    /// List elements must be <see cref="ISeries"/> interface types.
    /// </typeparam>
    /// <param name="actual">The actual list of series to be compared.</param>
    /// <param name="expected">The expected list of series to compare against.</param>
    /// <param name="precision">The precision profile to use for comparisons.</param>
    internal static void AssertEquals<T>(
        this IEnumerable<T> actual,
        IEnumerable<T> expected,
        Precision precision
    ) where T : ISeries
    {
        if (precision is Precision.Strict)
        {
            string errorMessage = $"Do not use '{precision}'";
            throw new ArgumentOutOfRangeException(nameof(precision), errorMessage);
        }

        // Map the profile to type-specific tolerances (absolute deltas)
        (double dTol, float fTol, decimal mTol) = GetTolerances(precision);

        actual.Should().BeEquivalentTo(expected, options => {
            EquivalencyAssertionOptions<T> o = options
                .WithStrictOrdering()
                .ComparingByMembers<T>();

            if (dTol > 0d)
            {
                o = o
                .Using<double>(ctx
                    => ctx.Subject.Should().BeApproximately(ctx.Expectation, dTol))
                .WhenTypeIs<double>()
                .Using<double?>(ctx
                    => ctx.Subject.Should().BeApproximately(ctx.Expectation, dTol))
                .WhenTypeIs<double?>();
            }

            if (fTol > 0f)
            {
                o = o
                .Using<float>(ctx
                    => ctx.Subject.Should().BeApproximately(ctx.Expectation, fTol))
                .WhenTypeIs<float>()
                .Using<float?>(ctx
                    => ctx.Subject.Should().BeApproximately(ctx.Expectation, fTol))
                .WhenTypeIs<float?>();
            }

            if (mTol > 0m)
            {
                o = o
                .Using<decimal>(ctx
                    => ctx.Subject.Should().BeApproximately(ctx.Expectation, mTol))
                .WhenTypeIs<decimal>()
                .Using<decimal?>(ctx
                    => ctx.Subject.Should().BeApproximately(ctx.Expectation, mTol))
                .WhenTypeIs<decimal?>();
            }

            return o;
        });
    }

    private static (double dTol, float fTol, decimal mTol) GetTolerances(Precision profile) =>
        // Choose conservative defaults. Doubles get the primary tolerance; decimal remains strict for
        // LastDigit to avoid masking financial rounding issues. Adjust profiles as needed per repository standards.
        profile switch {
            Precision.Strict => (0d, 0f, 0m),

            // Allow last-digit wiggle on doubles; floats get a proportionate small tolerance; decimals strict
            Precision.LastDigit => (1e-12d, 1e-5f, 0m),

            // Slightly more forgiving across types
            Precision.Tolerant => (1e-9d, 1e-4f, 1e-9m),

            // Most lenient — use sparingly
            Precision.Loose => (1e-6d, 1e-3f, 1e-6m),

            _ => (0d, 0f, 0m)
        };
}
