using System.Reflection;
using Skender.Stock.Indicators;
using Tests.Data;

namespace BaselineGenerator;

/// <summary>
/// Executes indicators and captures their output for baseline generation.
/// </summary>
internal static class IndicatorExecutor
{
    private static readonly IReadOnlyList<Quote> TestData = Data.GetDefault();

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

        // Check if this indicator uses SeriesParameter (requires IReusable lists) - not supported yet
        if (listing.Parameters != null && listing.Parameters.Any(p => p.DataType == "IReadOnlyList<T> where T : IReusable"))
        {
            throw new NotSupportedException($"Indicator '{listing.Uiid}' uses SeriesParameter which is not yet supported for baseline generation");
        }

        // Find all types in the indicators assembly
        Assembly indicatorsAssembly = typeof(Quote).Assembly;
        
        // Look for extension methods across all types
        MethodInfo? method = null;
        foreach (Type type in indicatorsAssembly.GetTypes())
        {
            method = type
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == methodName && m.IsGenericMethod);
            
            if (method != null)
            {
                break;
            }
        }

        if (method == null)
        {
            throw new InvalidOperationException($"Method '{methodName}' not found for indicator '{listing.Uiid}'");
        }

        // Make generic method with Quote type
        MethodInfo genericMethod = method.MakeGenericMethod(typeof(Quote));

        // Prepare parameters
        object?[] parameters = PrepareParameters(listing, genericMethod);

        // Invoke the method
        try
        {
            object? result = genericMethod.Invoke(null, parameters);

            if (result == null)
            {
                throw new InvalidOperationException($"Indicator '{listing.Uiid}' returned null result");
            }

            return result;
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

    private static object?[] PrepareParameters(IndicatorListing listing, MethodInfo method)
    {
        ParameterInfo[] methodParams = method.GetParameters();
        
        // First parameter is always the quotes collection
        List<object?> parameters = [TestData];

        // Add additional parameters from the listing with their default values
        if (listing.Parameters != null && listing.Parameters.Count > 0)
        {
            foreach (IndicatorParam param in listing.Parameters)
            {
                parameters.Add(param.DefaultValue);
            }
        }

        return parameters.ToArray();
    }
}
