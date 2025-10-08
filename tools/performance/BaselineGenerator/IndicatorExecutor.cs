using System.Reflection;
using Skender.Stock.Indicators;
using Test.Data;

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
        // Determine the folder range based on indicator name
        char firstLetter = char.ToLowerInvariant(listing.Uiid[0]);
        string folderRange = firstLetter switch
        {
            >= 'a' and <= 'd' => "a-d",
            >= 'e' and <= 'k' => "e-k",
            >= 'm' and <= 'r' => "m-r",
            >= 's' and <= 'z' => "s-z",
            _ => "other"
        };

        // Find the actual directory name by looking for test files
        // This handles the case where UIID might be uppercase but directory is PascalCase
        string rangeDir = Path.Combine(Directory.GetCurrentDirectory(), folderRange);
        
        string? actualIndicatorDir = null;
        if (Directory.Exists(rangeDir))
        {
            // Look for directories that match the indicator name (case-insensitive)
            foreach (string dir in Directory.GetDirectories(rangeDir))
            {
                string dirName = Path.GetFileName(dir);
                if (string.Equals(dirName, listing.Uiid, StringComparison.OrdinalIgnoreCase))
                {
                    // Verify this directory has test files
                    if (Directory.GetFiles(dir, "*.Tests.cs").Length > 0)
                    {
                        actualIndicatorDir = dirName;
                        break;
                    }
                }
            }
        }

        // If not found, use the UIID as-is (will create new directory if needed)
        string indicatorDirName = actualIndicatorDir ?? listing.Uiid;

        // Build path: {current_dir}/{range}/{IndicatorName}/{IndicatorName}.Baseline.json
        string testRoot = Path.Combine(
            Directory.GetCurrentDirectory(),
            folderRange,
            indicatorDirName);

        string fileName = $"{indicatorDirName}.Baseline.json";
        return Path.GetFullPath(Path.Combine(testRoot, fileName));
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
