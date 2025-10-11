using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
using FluentAssertions.Numeric;

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
    /// Asserts that two <see cref="IEnumerable{T}" /> series are equivalent with a precision profile.
    /// </summary>
    /// <typeparam name="T">List elements must be <see cref="ISeries"/> interface types.</typeparam>
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

                // Non-nullable double: consider NaN == NaN under precision; else approximate
                .Using<double>(ctx => {

                    if (double.IsNaN(ctx.Subject) && double.IsNaN(ctx.Expectation))
                    {
                        return;
                    }

                    ctx.Subject.Should().BeApproximately(ctx.Expectation, dTol);
                }).WhenTypeIs<double>()

                // Nullable double: handle nulls, then NaN == NaN, else approximate
                .Using<double?>(ctx => {

                    if (ctx.Subject is null || ctx.Expectation is null)
                    {
                        ctx.Subject.Should().Be(ctx.Expectation);
                        return;
                    }

                    double s = ctx.Subject.Value;
                    double e = ctx.Expectation.Value;
                    if (double.IsNaN(s) && double.IsNaN(e))
                    {
                        return;
                    }

                    s.Should().BeApproximately(e, dTol);
                }).WhenTypeIs<double?>();
            }

            if (fTol > 0f)
            {
                o = o

                // Non-nullable float: consider NaN == NaN; else approximate
                .Using<float>(ctx => {
                    if (float.IsNaN(ctx.Subject) && float.IsNaN(ctx.Expectation))
                    {
                        return;
                    }

                    ctx.Subject.Should().BeApproximately(ctx.Expectation, fTol);
                }).WhenTypeIs<float>()

                // Nullable float: handle nulls, then NaN == NaN, else approximate
                .Using<float?>(ctx => {
                    if (ctx.Subject is null || ctx.Expectation is null)
                    {
                        ctx.Subject.Should().Be(ctx.Expectation);
                        return;
                    }

                    float s = ctx.Subject.Value;
                    float e = ctx.Expectation.Value;
                    if (float.IsNaN(s) && float.IsNaN(e))
                    {
                        return;
                    }

                    s.Should().BeApproximately(e, fTol);
                }).WhenTypeIs<float?>();
            }

            if (mTol > 0m)
            {
                o = o

                .Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, mTol))
                .WhenTypeIs<decimal>()

                .Using<decimal?>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, mTol))
                .WhenTypeIs<decimal?>();
            }

            return o;
        });
    }

    /// <summary>
    /// Get the tolerance (decimal places) values for a given precision profile.
    /// </summary>
    /// <remarks>
    /// Choose conservative defaults.
    /// Doubles get the primary tolerance; decimal remains strict for
    /// <see cref="Precision.LastDigit"/> to avoid masking financial rounding issues.
    /// Adjust profiles as needed per repository standards.
    /// </remarks>
    private static (double dTol, float fTol, decimal mTol) GetTolerances(Precision profile)
        => profile switch {
            Precision.Strict => (0d, 0f, 0m),

            // Allow last-digit wiggle on doubles; floats get a proportionate small tolerance; decimals strict
            Precision.LastDigit => (1e-13d, 1e-8f, 0m),

            // Slightly more forgiving across types
            Precision.Tolerant => (1e-10d, 1e-5f, 1e-18m),

            // Most lenient — use sparingly
            Precision.Loose => (1e-8d, 1e-3f, 1e-10m),

            _ => (0d, 0f, 0m)
        };
}


// using directives moved to top of file; duplicate removed

/// <summary>
/// FluentAssertions extensions for ULP (Units in Last Place) based floating-point comparisons.
/// </summary>
public static class FluentAssertionExtensions
{
    /// <summary>
    /// Checks whether two doubles are within N ULPs (Units in Last Place) using the IEEE-754 helpers built into System.Double.
    /// </summary>
    /// <param name="assertions">The double assertions context.</param>
    /// <param name="expected">The expected double value.</param>
    /// <param name="maxUlps">Maximum allowed ULPs difference (default: 1).</param>
    /// <returns>AndConstraint for method chaining.</returns>
    /// <remarks>
    /// ULP-based comparison accounts for the varying precision across the range of representable values,
    /// making it more mathematically sound than absolute tolerance for floating-point comparisons.
    /// Ref: https://learn.microsoft.com/en-us/dotnet/api/system.double
    /// </remarks>
    [CLSCompliant(false)]
    public static AndConstraint<NumericAssertions<double>> BeApproximatelyUlps(
        this NumericAssertions<double> assertions,
        double expected,
        int maxUlps = 1)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        double actual = assertions.Subject.GetValueOrDefault();

        // Handle special cases: NaN, infinity
        if (double.IsNaN(actual) && double.IsNaN(expected))
        {
            return new AndConstraint<NumericAssertions<double>>(assertions);
        }

        if (!double.IsFinite(actual) || !double.IsFinite(expected))
        {
            Execute.Assertion
                .ForCondition(actual.Equals(expected))
                .FailWith("Expected {0} to equal {1} when comparing non-finite values.", actual, expected);
            return new AndConstraint<NumericAssertions<double>>(assertions);
        }

        // Fast exit for exact equality (handles +0 vs -0)
        if (actual == expected)
        {
            return new AndConstraint<NumericAssertions<double>>(assertions);
        }

        // Calculate ULP distance
        int ulps = CalculateUlpDistance(actual, expected);

        Execute.Assertion
            .ForCondition(ulps <= maxUlps)
            .FailWith("Expected {0} to be within {1} ULPs of {2}, but difference is {3} ULPs.",
                      actual, maxUlps, expected, ulps);

        return new AndConstraint<NumericAssertions<double>>(assertions);
    }

    /// <summary>
    /// Checks whether two floats are within N ULPs using BitIncrement/BitDecrement for float precision.
    /// </summary>
    /// <param name="assertions">The float assertions context.</param>
    /// <param name="expected">The expected float value.</param>
    /// <param name="maxUlps">Maximum allowed ULPs difference (default: 1).</param>
    /// <returns>AndConstraint for method chaining.</returns>
    [CLSCompliant(false)]
    public static AndConstraint<NumericAssertions<float>> BeApproximatelyUlps(
        this NumericAssertions<float> assertions,
        float expected,
        int maxUlps = 1)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        float actual = assertions.Subject.GetValueOrDefault();

        // Handle special cases: NaN, infinity
        if (float.IsNaN(actual) && float.IsNaN(expected))
        {
            return new AndConstraint<NumericAssertions<float>>(assertions);
        }

        if (!float.IsFinite(actual) || !float.IsFinite(expected))
        {
            Execute.Assertion
                .ForCondition(actual.Equals(expected))
                .FailWith("Expected {0} to equal {1} when comparing non-finite values.", actual, expected);
            return new AndConstraint<NumericAssertions<float>>(assertions);
        }

        // Fast exit for exact equality (handles +0 vs -0)
        if (actual == expected)
        {
            return new AndConstraint<NumericAssertions<float>>(assertions);
        }

        // Calculate ULP distance for float
        int ulps = CalculateUlpDistance(actual, expected);

        Execute.Assertion
            .ForCondition(ulps <= maxUlps)
            .FailWith("Expected {0} to be within {1} ULPs of {2}, but difference is {3} ULPs.",
                      actual, maxUlps, expected, ulps);

        return new AndConstraint<NumericAssertions<float>>(assertions);
    }

    /// <summary>
    /// Calculate the ULP distance between two double values.
    /// </summary>
    private static int CalculateUlpDistance(double actual, double expected)
    {
        double next = actual;
        int ulps = 0;
        const int maxIterations = 1000; // Prevent infinite loops

        if (actual < expected)
        {
            while (next < expected && ulps < maxIterations)
            {
                next = double.BitIncrement(next);
                ulps++;
            }
        }
        else
        {
            while (next > expected && ulps < maxIterations)
            {
                next = double.BitDecrement(next);
                ulps++;
            }
        }

        return ulps;
    }

    /// <summary>
    /// Calculate the ULP distance between two float values.
    /// </summary>
    private static int CalculateUlpDistance(float actual, float expected)
    {
        float next = actual;
        int ulps = 0;
        const int maxIterations = 1000; // Prevent infinite loops

        if (actual < expected)
        {
            while (next < expected && ulps < maxIterations)
            {
                next = float.BitIncrement(next);
                ulps++;
            }
        }
        else
        {
            while (next > expected && ulps < maxIterations)
            {
                next = float.BitDecrement(next);
                ulps++;
            }
        }

        return ulps;
    }
}

internal static partial class Utilities
{
    /// <summary>
    /// Assert structural equivalence using ULP-based floating-point comparison for doubles/floats
    /// with profiles defining max ULP differences. Decimals remain strict by design.
    /// </summary>
    internal static void AssertEqualsUlp<T>(
        this IEnumerable<T> actual,
        IEnumerable<T> expected,
        UlpPrecision ulpPrecision
    ) where T : ISeries
    {
        int maxDoubleUlps = ulpPrecision switch {
            UlpPrecision.Exact => 0,
            UlpPrecision.SingleStep => 1,
            UlpPrecision.NearExact => 4,
            UlpPrecision.Standard => 16,
            UlpPrecision.Loose => 64,
            _ => 0
        };

        int maxFloatUlps = ulpPrecision switch {
            UlpPrecision.Exact => 0,
            UlpPrecision.SingleStep => 1,
            UlpPrecision.NearExact => 4,
            UlpPrecision.Standard => 16,
            UlpPrecision.Loose => 64,
            _ => 0
        };

        actual.Should().BeEquivalentTo(expected, options => {
            EquivalencyAssertionOptions<T> o = options
                .WithStrictOrdering()
                .ComparingByMembers<T>();

            // Double (non-nullable)
            o = o.Using<double>(ctx => {
                if (double.IsNaN(ctx.Subject) && double.IsNaN(ctx.Expectation))
                {
                    return;
                }

                if (maxDoubleUlps == 0)
                {
                    ctx.Subject.Should().Be(ctx.Expectation);
                }
                else
                {
                    ctx.Subject.Should().BeApproximatelyUlps(ctx.Expectation, maxDoubleUlps);
                }
            }).WhenTypeIs<double>();

            // Double? (nullable)
            o = o.Using<double?>(ctx => {
                if (ctx.Subject is null || ctx.Expectation is null)
                {
                    ctx.Subject.Should().Be(ctx.Expectation);
                    return;
                }

                double s = ctx.Subject.Value;
                double e = ctx.Expectation.Value;
                if (double.IsNaN(s) && double.IsNaN(e))
                {
                    return;
                }

                if (maxDoubleUlps == 0)
                {
                    s.Should().Be(e);
                }
                else
                {
                    s.Should().BeApproximatelyUlps(e, maxDoubleUlps);
                }
            }).WhenTypeIs<double?>();

            // Float (non-nullable)
            o = o.Using<float>(ctx => {
                if (float.IsNaN(ctx.Subject) && float.IsNaN(ctx.Expectation))
                {
                    return;
                }

                if (maxFloatUlps == 0)
                {
                    ctx.Subject.Should().Be(ctx.Expectation);
                }
                else
                {
                    ctx.Subject.Should().BeApproximatelyUlps(ctx.Expectation, maxFloatUlps);
                }
            }).WhenTypeIs<float>();

            // Float? (nullable)
            o = o.Using<float?>(ctx => {
                if (ctx.Subject is null || ctx.Expectation is null)
                {
                    ctx.Subject.Should().Be(ctx.Expectation);
                    return;
                }

                float s = ctx.Subject.Value;
                float e = ctx.Expectation.Value;
                if (float.IsNaN(s) && float.IsNaN(e))
                {
                    return;
                }

                if (maxFloatUlps == 0)
                {
                    s.Should().Be(e);
                }
                else
                {
                    s.Should().BeApproximatelyUlps(e, maxFloatUlps);
                }
            }).WhenTypeIs<float?>();

            // Decimal remains strict (no extra rules here)

            return o;
        });
    }
}
