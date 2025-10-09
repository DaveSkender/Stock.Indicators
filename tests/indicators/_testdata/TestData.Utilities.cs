namespace TestsUtilities;

internal static class Utilities
{
    internal static void AssertEquals<T>(
        this IReadOnlyList<T> actual,
        IReadOnlyList<T> expected
    ) where T : ISeries
    => actual.Should().BeEquivalentTo(expected, options => options
        .WithStrictOrdering()
        .ComparingByMembers<T>()
        .WithTracing());
}
