using System.Reflection;
using System.Text.Json.Serialization;

namespace Tests.Indicators.Baselines;

#pragma warning disable CA1002 // Use generic collection type for public API compatibility with test infrastructure

/// <summary>
/// Compares indicator results against baseline files using exact binary equality.
/// Mathematical precision is NON-NEGOTIABLE per Constitution Principle I.
/// </summary>
public static class BaselineComparer
{
    /// <summary>
    /// Compares two sequences of indicator results using exact binary equality.
    /// </summary>
    /// <typeparam name="TResult">The indicator result type</typeparam>
    /// <param name="expected">Expected results from baseline</param>
    /// <param name="actual">Actual results from indicator execution</param>
    /// <returns>Comparison result with match status and mismatch details</returns>
    public static ComparisonResult Compare<TResult>(
        IEnumerable<TResult> expected,
        IEnumerable<TResult> actual)
    {
        List<MismatchDetail> mismatches = [];

        List<TResult> expectedList = expected.ToList();
        List<TResult> actualList = actual.ToList();

        // Check for count mismatch
        if (expectedList.Count != actualList.Count)
        {
            mismatches.Add(new MismatchDetail(
                Timestamp: DateTime.MinValue,
                PropertyName: "_COUNT_",
                Expected: expectedList.Count,
                Actual: actualList.Count,
                Delta: Math.Abs(expectedList.Count - actualList.Count)));

            return new ComparisonResult(false, mismatches);
        }

        // Compare each result in parallel by index
        for (int i = 0; i < expectedList.Count; i++)
        {
            TResult expectedItem = expectedList[i];
            TResult actualItem = actualList[i];

            // Get timestamp for error reporting
            PropertyInfo timestampProp = typeof(TResult).GetProperty("Timestamp")
                ?? typeof(TResult).GetProperty("Date")
                ?? throw new InvalidOperationException($"Result type {typeof(TResult).Name} must have Timestamp or Date property");

            DateTime timestamp = (DateTime)(timestampProp.GetValue(expectedItem) ?? DateTime.MinValue);

            // Compare all numeric properties (excluding JsonIgnore properties like Value)
            PropertyInfo[] properties = typeof(TResult).GetProperties()
                .Where(p => (p.PropertyType == typeof(double?)
                          || p.PropertyType == typeof(double)
                          || p.PropertyType == typeof(decimal?)
                          || p.PropertyType == typeof(decimal))
                       && p.GetCustomAttribute<JsonIgnoreAttribute>() is null)
                .ToArray();

            foreach (PropertyInfo prop in properties)
            {
                // Get values - null coalescing to sentinel object for type checking
                object expectedValueObj = prop.GetValue(expectedItem);
                object actualValueObj = prop.GetValue(actualItem);

                // Handle null comparisons
                if (expectedValueObj is null && actualValueObj is null)
                {
                    continue; // Both null, match
                }

                if (expectedValueObj is null || actualValueObj is null)
                {
                    // One is null, the other isn't
                    mismatches.Add(new MismatchDetail(
                        Timestamp: timestamp,
                        PropertyName: prop.Name,
                        Expected: ConvertToNullableDouble(expectedValueObj),
                        Actual: ConvertToNullableDouble(actualValueObj),
                        Delta: null));
                    continue;
                }

                // Exact binary equality check (NON-NEGOTIABLE)
                bool areEqual = expectedValueObj switch
                {
                    double expDbl when actualValueObj is double actDbl => expDbl == actDbl,
                    decimal expDec when actualValueObj is decimal actDec => expDec == actDec,
                    _ => false
                };

                if (!areEqual)
                {
                    double? expDbl = ConvertToNullableDouble(expectedValueObj);
                    double? actDbl = ConvertToNullableDouble(actualValueObj);
                    double? delta = expDbl.HasValue && actDbl.HasValue
                        ? Math.Abs(expDbl.Value - actDbl.Value)
                        : null;

                    mismatches.Add(new MismatchDetail(
                        Timestamp: timestamp,
                        PropertyName: prop.Name,
                        Expected: expDbl,
                        Actual: actDbl,
                        Delta: delta));
                }
            }
        }

        bool isMatch = mismatches.Count == 0;
        return new ComparisonResult(isMatch, mismatches);
    }

    private static double? ConvertToNullableDouble(object value)
    {
        return value switch
        {
            null => null,
            double d => d,
            decimal m => (double)m,
            _ => null
        };
    }
}

/// <summary>
/// Result of a baseline comparison.
/// </summary>
/// <param name="IsMatch">True if results match exactly, false otherwise</param>
/// <param name="Mismatches">List of detected mismatches</param>
public record ComparisonResult(
    bool IsMatch,
    List<MismatchDetail> Mismatches);

/// <summary>
/// Details of a single property mismatch.
/// </summary>
/// <param name="Timestamp">The timestamp/date of the mismatched result</param>
/// <param name="PropertyName">Name of the property that doesn't match</param>
/// <param name="Expected">Expected value from baseline</param>
/// <param name="Actual">Actual value from indicator execution</param>
/// <param name="Delta">Absolute difference between expected and actual</param>
public record MismatchDetail(
    DateTime Timestamp,
    string PropertyName,
    double? Expected,
    double? Actual,
    double? Delta);
