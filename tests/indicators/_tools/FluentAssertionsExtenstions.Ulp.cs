using FluentAssertions.Execution;
using FluentAssertions.Numeric;

namespace Tests.Tools;

/// <summary>
/// FluentAssertions extensions for ULP (Units in Last Place) based floating-point comparisons.
/// </summary>
public static class FluentAssertionUlpExtensions
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
    /// <param name="actual">Actual value</param>
    /// <param name="expected">Expected value</param>
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
    /// <param name="actual">Actual value</param>
    /// <param name="expected">Expected value</param>
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
