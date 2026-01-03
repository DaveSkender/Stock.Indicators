using System.Reflection;
using Skender.Stock.Indicators;

namespace Test.DataGenerator;

/// <summary>
/// Executes indicators and captures their output for baseline generation.
/// </summary>
internal static class IndicatorExecutor
{
    private static readonly IReadOnlyList<Quote> TestData = Test.Data.Data.GetDefault();
    private static readonly IReadOnlyList<Quote> OtherData = Test.Data.Data.GetCompare();

    /// <summary>
    /// Executes an indicator and returns its results.
    /// </summary>
    /// <param name="listing">The indicator listing to execute.</param>
    /// <returns>The indicator results as a list.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the indicator method cannot be found or executed.</exception>
    public static object Execute(IndicatorListing listing)
    {
        // Get the method name from the listing
        string? methodName = listing.MethodName;

        if (string.IsNullOrEmpty(methodName))
        {
            throw new InvalidOperationException($"Method name not specified for indicator '{listing.Uiid}'");
        }

        // Check if this indicator uses SeriesParameter (requires dual quote lists)
        bool isDualInput = listing.Parameters?.Any(p => p.DataType == "IReadOnlyList<T> where T : IReusable") == true;

        // Find all types in the indicators assembly
        Assembly indicatorsAssembly = typeof(Quote).Assembly;

        // Calculate expected parameter count for method lookup
        // Extension source (this) counts as parameter 1
        // For dual-input: first series param is extension, second series param is explicit
        int expectedParamCount = 1; // Extension source
        bool firstSeriesParamSeen = false;
        if (listing.Parameters?.Count > 0)
        {
            foreach (IndicatorParam param in listing.Parameters)
            {
                if (param.DataType == "IReadOnlyList<T> where T : IReusable")
                {
                    if (!firstSeriesParamSeen)
                    {
                        // First series param is the extension source, already counted
                        firstSeriesParamSeen = true;
                        continue;
                    }

                    // Second series param is an explicit parameter
                    expectedParamCount++;
                }
                else
                {
                    // Non-series parameters are explicit
                    expectedParamCount++;
                }
            }
        }

        // Look for extension methods across all types
        MethodInfo? method = null;
        foreach (Type type in indicatorsAssembly.GetTypes())
        {
            method = type
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == methodName && m.IsGenericMethod && m.GetParameters().Length == expectedParamCount);

            if (method != null)
            {
                break;
            }
        }

        // If no generic method found, try to find a non-generic method with correct parameter count
        if (method == null)
        {
            foreach (Type type in indicatorsAssembly.GetTypes())
            {
                method = type
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m => m.Name == methodName && !m.IsGenericMethod && m.GetParameters().Length == expectedParamCount);

                if (method != null)
                {
                    break;
                }
            }
        }

        // Fallback: try any matching method if exact match not found
        if (method == null)
        {
            foreach (Type type in indicatorsAssembly.GetTypes())
            {
                method = type
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m => m.Name == methodName);

                if (method != null)
                {
                    break;
                }
            }
        }

        if (method == null)
        {
            throw new InvalidOperationException($"Method '{methodName}' not found for indicator '{listing.Uiid}'");
        }

        // Make generic method with Quote type if it's generic, otherwise use method directly
        MethodInfo targetMethod = method.IsGenericMethod
            ? method.MakeGenericMethod(typeof(Quote))
            : method;

        // Prepare parameters
        object?[] parameters = PrepareParameters(listing, targetMethod, isDualInput);

        // Invoke the method
        try
        {
            object? result = targetMethod.Invoke(null, parameters);

            return result
                ?? throw new InvalidOperationException($"Indicator '{listing.Uiid}' returned null result");
        }
        catch (TargetInvocationException ex)
        {
            // Unwrap the inner exception for clearer error messages
            if (ex.InnerException != null)
            {
                throw ex.InnerException;
            }

            throw;
        }
    }

    /// <summary>
    /// Gets the baseline file path for an indicator.
    /// </summary>
    /// <param name="listing">The indicator listing.</param>
    /// <returns>The full path to the baseline file.</returns>
    public static string GetBaselinePath(IndicatorListing listing)
    {
        // Baselines are stored in _testdata/results/ directory
        // Filename pattern: {uiid-lowercase}.standard.json
        string resultsDir = Path.Combine(
            Directory.GetCurrentDirectory(),
            "_testdata",
            "results");

        // Create results directory if it doesn't exist
        if (!Directory.Exists(resultsDir))
        {
            Directory.CreateDirectory(resultsDir);
        }

        // Use lowercase UIID with hyphen format (e.g., "SMA" -> "sma.standard.json")
        string fileName = $"{listing.Uiid.ToLowerInvariant()}.standard.json";
        return Path.GetFullPath(Path.Combine(resultsDir, fileName));
    }

    private static object?[] PrepareParameters(IndicatorListing listing, MethodInfo method, bool isDualInput)
    {
        ParameterInfo[] methodParams = method.GetParameters();

        // For dual-input indicators, first parameter is OtherData (extension source)
        // For single-input indicators, first parameter is TestData
        List<object?> parameters = isDualInput ? new List<object?>() { OtherData } : new List<object?>() { TestData };

        // Add additional parameters from the listing with their default values
        if (listing.Parameters?.Count > 0)
        {
            bool firstSeriesParamSkipped = false;
            int paramIndex = 1; // Start at 1 because first param is quotes (extension source)

            foreach (IndicatorParam param in listing.Parameters)
            {
                // For dual-input indicators, the first series parameter is the extension source (already added)
                // The second series parameter is the comparison source
                if (param.DataType == "IReadOnlyList<T> where T : IReusable")
                {
                    if (isDualInput && !firstSeriesParamSkipped)
                    {
                        // Skip the first series parameter (it's the extension source already added)
                        firstSeriesParamSkipped = true;
                        continue;
                    }
                    // Add the second series parameter (the comparison source)
                    parameters.Add(TestData);
                    paramIndex++;

                    continue;
                }

                object? value = param.DefaultValue;

                // Special handling for required parameters with no default
                if (param.IsRequired && value == null && paramIndex < methodParams.Length)
                {
                    ParameterInfo methodParam = methodParams[paramIndex];

                    // For DateTime parameters (like VWAP startDate), use the first quote date
                    if (methodParam.ParameterType == typeof(DateTime))
                    {
                        value = TestData[0].Timestamp;
                    }
                }

                // Handle type conversions if needed
                if (value != null && paramIndex < methodParams.Length)
                {
                    ParameterInfo methodParam = methodParams[paramIndex];
                    Type targetType = methodParam.ParameterType;

                    // Convert double to decimal if needed (for RENKO, ZIGZAG, etc.)
                    if (value is double doubleValue && targetType == typeof(decimal))
                    {
                        value = (decimal)doubleValue;
                    }
                }

                parameters.Add(value);
                paramIndex++;
            }
        }

        return parameters.ToArray();
    }
}
