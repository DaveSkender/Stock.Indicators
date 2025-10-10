namespace Tests.Data;

internal static partial class Utilities
{
    internal static void AssertEquals<T>(
        this IReadOnlyList<T> actual,
        IReadOnlyList<T> expected
    ) where T : ISeries
    => actual.Should().BeEquivalentTo(expected, options => options
        .WithStrictOrdering()
        .ComparingByMembers<T>()
        .Excluding(ctx => ctx.Name == "Date") // Exclude obsolete Date property (alias for Timestamp)
        .WithTracing());

    internal static void AssertEquals<T>(
        this IReadOnlyList<T> actual,
        IReadOnlyList<T> expected,
        double precision
    ) where T : ISeries
    => actual.Should().BeEquivalentTo(expected, options => options
        .WithStrictOrdering()
        .ComparingByMembers<T>()
        .Excluding(ctx => ctx.Name == "Date") // Exclude obsolete Date property (alias for Timestamp)
        .Using<double>(ctx => {
            if (double.IsNaN(ctx.Subject) && double.IsNaN(ctx.Expectation))
            {
                // Both NaN - consider equal
                return;
            }
            else if (double.IsNaN(ctx.Subject) || double.IsNaN(ctx.Expectation))
            {
                // One is NaN, the other isn't - fail
                ctx.Subject.Should().Be(ctx.Expectation);
            }
            else
            {
                // Both are normal numbers - use approximate comparison
                ctx.Subject.Should().BeApproximately(ctx.Expectation, precision);
            }
        })
        .WhenTypeIs<double>()
        .Using<double?>(ctx => {
            if (ctx.Subject.HasValue && ctx.Expectation.HasValue)
            {
                if (double.IsNaN(ctx.Subject.Value) && double.IsNaN(ctx.Expectation.Value))
                {
                    // Both NaN - consider equal
                    return;
                }
                else if (double.IsNaN(ctx.Subject.Value) || double.IsNaN(ctx.Expectation.Value))
                {
                    // One is NaN, the other isn't - fail
                    ctx.Subject.Should().Be(ctx.Expectation);
                }
                else
                {
                    // Both are normal numbers - use approximate comparison
                    ctx.Subject.Value.Should().BeApproximately(ctx.Expectation.Value, precision);
                }
            }
            else
            {
                ctx.Subject.Should().Be(ctx.Expectation);
            }
        })
        .WhenTypeIs<double?>()
        .WithTracing());
}
