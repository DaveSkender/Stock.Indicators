using static Tests.Data.Utilities;

namespace Tests.Utilities;

[TestClass]
[TestCategory("Utilities")]
public class UtilitiesPrecisionTests
{
    private static DateTime T(int i) => new DateTime(2020, 1, 1).AddDays(i);

    private static IReadOnlyList<TestSeries> Series(
        double? d = null,
        float? f = null,
        decimal? m = null,
        int count = 1)
    {
        return Enumerable.Range(0, count)
            .Select(i => new TestSeries(T(i)) {
                D = d,
                F = f,
                M = m
            })
            .ToList();
    }

    private record TestSeries(DateTime Timestamp) : ISeries
    {
        public double? D { get; init; }
        public float? F { get; init; }
        public decimal? M { get; init; }
    }

    [TestMethod]
    public void NoProfile_ExactMatchRequired()
    {
        // exact match passes with no precision profile
        IReadOnlyList<TestSeries> actual = Series(d: 1.23456789);
        IReadOnlyList<TestSeries> expected = Series(d: 1.23456789);
        actual.AssertEquals(expected);

        // tiny difference fails without a profile
        IReadOnlyList<TestSeries> expected2 = Series(d: 1.23456789000001);
        Action act = () => actual.AssertEquals(expected2);
        act.Should().Throw<Exception>();
    }

    [TestMethod]
    public void Strict_ThrowsWhenRequested()
    {
        // Strict is intentionally disabled for the precision overload; use the no-profile overload instead
        IReadOnlyList<TestSeries> actual = Series(d: 1.0);
        IReadOnlyList<TestSeries> expected = Series(d: 1.0);

        Action act = () => actual.AssertEquals(expected, Precision.Strict);
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Do not use 'Strict'*");
    }

    [TestMethod]
    public void LastDigit_AllowsDoubleLastDigit()
    {
        // LastDigit allows tiny last-digit wobble for double
        IReadOnlyList<TestSeries> actual = Series(d: 262.18636765371303);
        IReadOnlyList<TestSeries> expected = Series(d: 262.1863676537131);

        actual.AssertEquals(expected, Precision.LastDigit);

        // but larger differences should still fail under LastDigit
        IReadOnlyList<TestSeries> expectedFail = Series(d: 262.1863677537131); // ~1e-7 delta
        Action act = () => actual.AssertEquals(expectedFail, Precision.LastDigit);
        act.Should().Throw<Exception>();
    }

    [TestMethod]
    public void LastDigit_TreatsNaNEqual_ForFloatAndDouble()
    {
        // NaN vs NaN should be considered equivalent under precision mode for float/double
        IReadOnlyList<TestSeries> actual = Series(d: double.NaN, f: float.NaN);
        IReadOnlyList<TestSeries> expected = Series(d: double.NaN, f: float.NaN);

        actual.AssertEquals(expected, Precision.LastDigit);

        // NaN vs value should still fail
        IReadOnlyList<TestSeries> expectedFail = Series(d: 1.0, f: 1.0f);
        Action act = () => actual.AssertEquals(expectedFail, Precision.LastDigit);
        act.Should().Throw<Exception>();
    }

    [TestMethod]
    public void LastDigit_LeavesDecimalStrict()
    {
        // decimal remains strict under LastDigit — even a tiny difference should fail
        IReadOnlyList<TestSeries> actual = Series(m: 1.000000000000000000m);
        IReadOnlyList<TestSeries> expected = Series(m: 1.000000000000000001m);

        Action act = () => actual.AssertEquals(expected, Precision.LastDigit);
        act.Should().Throw<Exception>();
    }

    [TestMethod]
    public void Tolerant_PermitsSmallDoubleDifference()
    {
        // Under current mapping, Tolerant allows ~1e-10 for double
        IReadOnlyList<TestSeries> actual = Series(d: 1.0000000000);
        IReadOnlyList<TestSeries> expectedPass = Series(d: 1.00000000001); // diff ~1e-10 (boundary)
        IReadOnlyList<TestSeries> expectedFail = Series(d: 1.0000000001);  // diff ~1e-9  (too large)

        actual.AssertEquals(expectedPass, Precision.Tolerant);
        Action act = () => actual.AssertEquals(expectedFail, Precision.Tolerant);
        act.Should().Throw<Exception>();
    }

    [TestMethod]
    public void Loose_PermitsLargerFloatDifference()
    {
        // Loose allows a larger tolerance for float; use a delta that fails Tolerant but passes Loose
        IReadOnlyList<TestSeries> actual = Series(f: 1.0f);
        IReadOnlyList<TestSeries> expected = Series(f: 1.0f + 5e-5f); // 5e-5 > 1e-5 (Tolerant), but < 1e-3 (Loose)

        // sanity: it should fail Tolerant
        Action actTolerant = () => actual.AssertEquals(expected, Precision.Tolerant);
        actTolerant.Should().Throw<Exception>();

        // and pass Loose
        actual.AssertEquals(expected, Precision.Loose);
    }
}
