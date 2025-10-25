using static Tests.Data.Utilities;

namespace Tests.Utilities;

[TestClass]
[TestCategory("Utilities")]
public class UtilitiesUlpPrecisionTests
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
    public void Ulp_Exact_RequiresExactEquality_Double()
    {
        const double expected = 1.0;
        double actualOneUlp = double.BitIncrement(expected);

        var actual = Series(d: actualOneUlp);
        var expectedList = Series(d: expected);

        Action act = () => actual.AssertEqualsUlp(expectedList, UlpPrecision.Exact);
        act.Should().Throw<Exception>();

        // exact should pass
        Series(d: expected).AssertEqualsUlp(expectedList, UlpPrecision.Exact);
    }

    [TestMethod]
    public void Ulp_SingleStep_Allows1Ulp_Double()
    {
        const double expected = 1000.0;
        double actualOneUlp = double.BitIncrement(expected);

        var actual = Series(d: actualOneUlp);
        var expectedList = Series(d: expected);

        actual.AssertEqualsUlp(expectedList, UlpPrecision.SingleStep);
    }

    [TestMethod]
    public void Ulp_SingleStep_Allows1Ulp_Float()
    {
        const float expected = 1000f;
        float actualOneUlp = float.BitIncrement(expected);

        var actual = Series(f: actualOneUlp);
        var expectedList = Series(f: expected);

        actual.AssertEqualsUlp(expectedList, UlpPrecision.SingleStep);
    }

    [TestMethod]
    public void Ulp_TreatsNaNEqual_ForFloatAndDouble()
    {
        var actual = Series(d: double.NaN, f: float.NaN);
        var expected = Series(d: double.NaN, f: float.NaN);

        actual.AssertEqualsUlp(expected, UlpPrecision.Standard);

        var expectedFail = Series(d: 1.0, f: 1.0f);
        Action act = () => actual.AssertEqualsUlp(expectedFail, UlpPrecision.Standard);
        act.Should().Throw<Exception>();
    }

    [TestMethod]
    public void Ulp_Infinity_MustMatchExactly()
    {
        var posInf = Series(d: double.PositiveInfinity, f: float.PositiveInfinity);
        var posInf2 = Series(d: double.PositiveInfinity, f: float.PositiveInfinity);
        posInf.AssertEqualsUlp(posInf2, UlpPrecision.Standard);

        var negInf = Series(d: double.NegativeInfinity, f: float.NegativeInfinity);
        Action act = () => posInf.AssertEqualsUlp(negInf, UlpPrecision.Standard);
        act.Should().Throw<Exception>();
    }

    [TestMethod]
    public void Ulp_Nullable_And_Decimal_Strict()
    {
        // Nullable equal and null handling
        var a1 = new[] { new TestSeries(T(0)) { D = null, F = null, M = 1.000000000000000000m } };
        var e1 = new[] { new TestSeries(T(0)) { D = null, F = null, M = 1.000000000000000000m } };
        a1.AssertEqualsUlp(e1, UlpPrecision.Standard);

        var a2 = new[] { new TestSeries(T(0)) { D = null } };
        var e2 = new[] { new TestSeries(T(0)) { D = 1.0 } };
        Action actNull = () => a2.AssertEqualsUlp(e2, UlpPrecision.Standard);
        actNull.Should().Throw<Exception>();

        // Decimal remains strict under ULP comparisons
        var a3 = Series(m: 1.000000000000000000m);
        var e3 = Series(m: 1.000000000000000001m);
        Action actDec = () => a3.AssertEqualsUlp(e3, UlpPrecision.Standard);
        actDec.Should().Throw<Exception>();
    }
}
