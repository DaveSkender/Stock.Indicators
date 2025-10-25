using System.Reflection;

namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for dynamic indicator execution based on catalog metadata.
/// </summary>
internal static class ListingExecutor
{
    // TODO: address proper use of generic vs interface TQuote and TResult

    /// <summary>
    /// Executes an indicator method dynamically using catalog metadata.
    /// </summary>
    /// <typeparam name="TQuote">The quote type implementing IQuote.</typeparam>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="quotes">The quotes to process.</param>
    /// <param name="listing">The indicator listing containing metadata.</param>
    /// <param name="parameters">Optional parameter overrides. If not provided, uses catalog default values.</param>
    /// <returns>The indicator results.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the indicator cannot be executed.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="quotes"/> is <c>null</c>.</exception>
    internal static IReadOnlyList<TResult> Execute<TQuote, TResult>(
        IEnumerable<IQuote> quotes,
        IndicatorListing listing,
        Dictionary<string, object>? parameters = null)  // TODO: is this redundant to listing.Parameters?
        where TResult : class
    {
        // Validate inputs
        if (quotes == null)
        {
            throw new ArgumentNullException(nameof(quotes));
        }

        if (listing == null)
        {
            throw new ArgumentNullException(nameof(listing));
        }

        string methodName = listing.MethodName
            ?? throw new InvalidOperationException("MethodName is required for dynamic execution");

        // Get the assembly containing the indicators
        Assembly indicatorsAssembly = typeof(Ema).Assembly;

        // Find all static classes in the assembly
        Type[] types = indicatorsAssembly.GetTypes()
            .Where(t => t.IsClass && t.IsAbstract && t.IsSealed) // static classes
            .ToArray();

        List<MethodInfo> methods = [];

        // Search for the method across all static classes
        foreach (Type type in types)
        {
            MethodInfo[] typeMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == methodName)
                .ToArray();
            methods.AddRange(typeMethods);
        }

        if (methods.Count == 0)
        {
            throw new InvalidOperationException($"Method {methodName} not found");
        }

        // Build parameter array using catalog metadata and user overrides
        List<object> parameterList = [quotes];

        // Add parameters based on catalog metadata
        if (listing.Parameters != null)
        {
            foreach (IndicatorParam param in listing.Parameters)
            {

                // Check if user provided an override
                if (parameters?.TryGetValue(param.ParameterName, out object? value) == true)
                {
                    parameterList.Add(value);
                }
                else if (param.IsRequired)
                {
                    // Use default value for required parameters
                    if (param.DefaultValue == null)
                    {
                        throw new InvalidOperationException(
                            $"Required parameter {param.ParameterName} has no default value and was not provided");
                    }

                    parameterList.Add(param.DefaultValue);
                }
                else
                {
                    // For optional parameters, use default value if available
                    if (param.DefaultValue != null)
                    {
                        parameterList.Add(param.DefaultValue);
                    }
                }
            }
        }

        // Find the method that matches our parameter count
        MethodInfo? targetMethod = methods.FirstOrDefault(m => m.GetParameters().Length == parameterList.Count) ?? throw new InvalidOperationException($"No {methodName} method found with {parameterList.Count} parameters");

        // If the method is generic, make it specific for the quote type
        if (targetMethod.IsGenericMethodDefinition)
        {
            Type[] genericArguments = targetMethod.GetGenericArguments();
            if (genericArguments.Length == 1)
            {
                targetMethod = targetMethod.MakeGenericMethod(typeof(TQuote));
            }
        }

        // Execute the method via reflection
        object? result = targetMethod.Invoke(null, parameterList.ToArray())
            ?? throw new InvalidOperationException("Method execution returned null");

        // Cast to expected type
        return result is IReadOnlyList<TResult> typedResult
            ? typedResult
            : throw new InvalidOperationException($"Result is not of expected type {typeof(IReadOnlyList<TResult>).Name}");
    }

    /// <summary>
    /// Executes an indicator method dynamically using catalog metadata with parameter values.
    /// This is a convenience method that creates the parameter dictionary automatically.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="quotes">The quotes to process.</param>
    /// <param name="listing">The indicator listing containing metadata.</param>
    /// <param name="parameterValues">Parameter values in the order they appear in the listing.</param>
    /// <returns>The indicator results.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="listing"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"></exception>
    internal static IReadOnlyList<TResult> Execute<TResult>(
        IEnumerable<IQuote> quotes,
        IndicatorListing listing,
        params object[] parameterValues)
        where TResult : class
    {
        // Validate inputs
        if (listing == null)
        {
            throw new ArgumentNullException(nameof(listing));
        }

        if (parameterValues == null)
        {
            throw new ArgumentNullException(nameof(parameterValues));
        }

        Dictionary<string, object>? parameters = null;

        if (parameterValues.Length > 0 && listing.Parameters != null)
        {
            if (parameterValues.Length > listing.Parameters.Count)
            {
                throw new ArgumentException($"Too many parameter values provided. Expected {listing.Parameters.Count}, got {parameterValues.Length}");
            }

            parameters = [];
            for (int i = 0; i < parameterValues.Length; i++)
            {
                parameters[listing.Parameters[i].ParameterName] = parameterValues[i];
            }
        }

        return Execute<IQuote, TResult>(quotes, listing, parameters);
    }
}
