using FluentAssertions.Equivalency;

namespace Tests.Tools;

/// <summary>
/// ULP-based assertion utilities for test validation.
/// </summary>
public static partial class TestAsserts
{
    /// <summary>
    /// Assert structural equivalence using ULP-based floating-point comparison for doubles/floats
    /// with profiles defining max ULP differences. Decimals remain strict by design.
    /// </summary>
    /// <param name="actual">Actual value</param>
    /// <param name="expected">Expected value</param>
    /// <param name="ulpPrecision">Units of precision</param>
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
