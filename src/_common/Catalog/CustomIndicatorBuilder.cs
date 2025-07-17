namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a customized indicator configuration that can be configured, sourced, and executed.
/// Supports fluent API for building indicator execution pipelines.
/// </summary>
public class CustomIndicatorBuilder
{
    private readonly Dictionary<string, object> _parameterOverrides;
    private IEnumerable<IQuote>? _quotes;

    internal CustomIndicatorBuilder(IndicatorListing baseListing, Dictionary<string, object>? parameterOverrides = null)
    {
        BaseListing = baseListing ?? throw new ArgumentNullException(nameof(baseListing));
        _parameterOverrides = parameterOverrides ?? [];
    }

    /// <summary>
    /// Sets or overrides a parameter value for the indicator.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to override.</param>
    /// <param name="value">The value to set for the parameter.</param>
    /// <returns>A new CustomIndicatorBuilder with the parameter override applied.</returns>
    public CustomIndicatorBuilder WithParamValue(string parameterName, object value)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentException("Parameter name cannot be null or empty", nameof(parameterName));
        }

        if (value != null)
        {
            ValidateParameterType(parameterName, value);
        }

        Dictionary<string, object> newOverrides = new(_parameterOverrides) {
            [parameterName] = value!
        };

        return new CustomIndicatorBuilder(BaseListing, newOverrides);
    }

    /// <summary>
    /// Sets multiple parameter values for the indicator.
    /// </summary>
    /// <param name="parameters">Dictionary of parameter names and values.</param>
    /// <returns>A new CustomIndicatorBuilder with the parameter overrides applied.</returns>
    public CustomIndicatorBuilder WithParams(Dictionary<string, object> parameters)
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        Dictionary<string, object> newOverrides = new(_parameterOverrides);
        foreach (KeyValuePair<string, object> kvp in parameters)
        {
            newOverrides[kvp.Key] = kvp.Value;
        }

        return new CustomIndicatorBuilder(BaseListing, newOverrides);
    }

    /// <summary>
    /// Specifies the source quotes for the indicator calculation.
    /// </summary>
    /// <param name="quotes">The quotes to process.</param>
    /// <returns>A new CustomIndicatorBuilder with the quotes set.</returns>
    public CustomIndicatorBuilder FromSource(IEnumerable<IQuote> quotes)
    {
        if (quotes == null)
        {
            throw new ArgumentNullException(nameof(quotes));
        }

        CustomIndicatorBuilder builder = new(BaseListing, _parameterOverrides) {
            _quotes = quotes
        };
        return builder;
    }

    /// <summary>
    /// Specifies a series source for the indicator calculation.
    /// This is equivalent to calling WithParamValue for a series parameter,
    /// but provides a more intuitive fluent API for chaining indicators.
    /// </summary>
    /// <typeparam name="T">The type of elements in the series, which must implement IReusable.</typeparam>
    /// <param name="series">The series data to process.</param>
    /// <param name="parameterName">The name of the series parameter. If null, attempts to find the first series parameter.</param>
    /// <returns>A new CustomIndicatorBuilder with the series parameter set.</returns>
    public CustomIndicatorBuilder FromSource<T>(IReadOnlyList<T> series, string? parameterName = null)
        where T : IReusable
    {
        if (series == null)
        {
            throw new ArgumentNullException(nameof(series));
        }

        // Find the appropriate series parameter
        string targetParam = parameterName ?? FindFirstSeriesParameter();

        return WithParamValue(targetParam, series);
    }

    /// <summary>
    /// Executes the configured indicator and returns the results.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <returns>The indicator results.</returns>
    /// <exception cref="InvalidOperationException">Thrown when quotes have not been set via From().</exception>
    public IReadOnlyList<TResult> Execute<TResult>()
        where TResult : class => _quotes == null
            ? throw new InvalidOperationException("Quotes must be set using From() before calling Execute()")
            : IndicatorExecutor.Execute<IQuote, TResult>(_quotes, BaseListing, _parameterOverrides);

    /// <summary>
    /// Validates that a parameter value is compatible with the expected parameter type.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">The value to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the parameter type is incompatible.</exception>
    private void ValidateParameterType(string parameterName, object value)
    {
        IndicatorParam? param = (BaseListing.Parameters?.FirstOrDefault(p => p.ParameterName == parameterName))
            ?? throw new ArgumentException($"Parameter '{parameterName}' not found in indicator '{BaseListing.Uiid}'", nameof(parameterName));

        // Validate series parameters
        if (param.DataType == "IReadOnlyList<T> where T : IReusable")
        {
            if (value is not System.Collections.IEnumerable)
            {
                throw new ArgumentException($"Parameter '{parameterName}' expects a series (IReadOnlyList<T> where T : IReusable), but received {value.GetType().Name}", nameof(value));
            }

            // Check if the series elements implement IReusable
            Type valueType = value.GetType();
            if ((valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>)) ||
                valueType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReadOnlyList<>)))
            {
                Type? elementType = valueType.GetGenericArguments().FirstOrDefault() ??
                                 valueType.GetInterfaces()
                                         .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReadOnlyList<>))
                                         ?.GetGenericArguments().FirstOrDefault();

                if (elementType != null && !typeof(IReusable).IsAssignableFrom(elementType))
                {
                    throw new ArgumentException($"Parameter '{parameterName}' expects series elements to implement IReusable, but received {elementType.Name}", nameof(value));
                }
            }
        }
        // Validate basic parameter types
        else if (param.DataType == "Int32" && value is not int)
        {
            throw new ArgumentException($"Parameter '{parameterName}' expects an integer value, but received {value.GetType().Name}", nameof(value));
        }
        else if ((param.DataType == "Double" || param.DataType == "Nullable`1") && value is not double && value is not int && value is not null) // Allow int to double conversion and nullable
        {
            throw new ArgumentException($"Parameter '{parameterName}' expects a double value, but received {value.GetType().Name}", nameof(value));
        }
        else if (param.DataType == "Boolean" && value is not bool)
        {
            throw new ArgumentException($"Parameter '{parameterName}' expects a boolean value, but received {value?.GetType().Name ?? "null"}", nameof(value));
        }
        // Additional type validations can be added here for other parameter types as needed
    }

    /// <summary>
    /// Finds the first series parameter in the indicator's parameter list.
    /// </summary>
    /// <returns>The name of the first series parameter.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no series parameter is found.</exception>
    private string FindFirstSeriesParameter()
    {
        IndicatorParam? seriesParam = BaseListing.Parameters?.FirstOrDefault(p => p.DataType == "IReadOnlyList<T> where T : IReusable");
        return seriesParam?.ParameterName ?? throw new InvalidOperationException($"No series parameter found in indicator '{BaseListing.Uiid}'");
    }

    /// <summary>
    /// Gets the base indicator listing.
    /// </summary>
    public IndicatorListing BaseListing { get; }

    /// <summary>
    /// Gets the parameter overrides.
    /// </summary>
    public IReadOnlyDictionary<string, object> ParameterOverrides => _parameterOverrides;

    /// <summary>
    /// Gets whether quotes have been set.
    /// </summary>
    public bool HasQuotes => _quotes != null;
}

/// <summary>
/// Extension methods for creating and working with custom indicator builders.
/// </summary>
public static class CustomIndicatorExtensions
{
    /// <summary>
    /// Creates a customizable indicator builder from an indicator listing.
    /// </summary>
    /// <param name="listing">The base indicator listing.</param>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns>A CustomIndicatorBuilder for fluent configuration.</returns>
    public static CustomIndicatorBuilder WithParamValue(this IndicatorListing listing, string parameterName, object value) => new CustomIndicatorBuilder(listing).WithParamValue(parameterName, value);

    /// <summary>
    /// Creates a customizable indicator builder from an indicator listing with multiple parameters.
    /// </summary>
    /// <param name="listing">The base indicator listing.</param>
    /// <param name="parameters">Dictionary of parameter names and values.</param>
    /// <returns>A CustomIndicatorBuilder for fluent configuration.</returns>
    public static CustomIndicatorBuilder WithParams(this IndicatorListing listing, Dictionary<string, object> parameters) => new CustomIndicatorBuilder(listing).WithParams(parameters);

    /// <summary>
    /// Creates a customizable indicator builder from an indicator listing and sets the source quotes.
    /// </summary>
    /// <param name="listing">The base indicator listing.</param>
    /// <param name="quotes">The quotes to process.</param>
    /// <returns>A CustomIndicatorBuilder with quotes set.</returns>
    public static CustomIndicatorBuilder From(this IndicatorListing listing, IEnumerable<IQuote> quotes) => new CustomIndicatorBuilder(listing).FromSource(quotes);

    /// <summary>
    /// Creates a customizable indicator builder from an indicator listing and sets a series source.
    /// </summary>
    /// <typeparam name="T">The type of elements in the series, which must implement IReusable.</typeparam>
    /// <param name="listing">The base indicator listing.</param>
    /// <param name="series">The series data to process.</param>
    /// <param name="parameterName">The name of the series parameter. If null, attempts to find the first series parameter.</param>
    /// <returns>A CustomIndicatorBuilder with the series parameter set.</returns>
    public static CustomIndicatorBuilder From<T>(this IndicatorListing listing, IReadOnlyList<T> series, string? parameterName = null)
        where T : IReusable => new CustomIndicatorBuilder(listing).FromSource(series, parameterName);

    /// <summary>
    /// Creates a customizable indicator builder from an indicator listing and sets a series source.
    /// Alias for From&lt;T&gt; to provide consistent naming with the fluent API.
    /// </summary>
    /// <typeparam name="T">The type of elements in the series, which must implement IReusable.</typeparam>
    /// <param name="listing">The base indicator listing.</param>
    /// <param name="series">The series data to process.</param>
    /// <param name="parameterName">The name of the series parameter. If null, attempts to find the first series parameter.</param>
    /// <returns>A CustomIndicatorBuilder with the series parameter set.</returns>
    public static CustomIndicatorBuilder FromSource<T>(this IndicatorListing listing, IReadOnlyList<T> series, string? parameterName = null)
        where T : IReusable => new CustomIndicatorBuilder(listing).FromSource(series, parameterName);

    /// <summary>
    /// Executes an indicator using the default parameters from the catalog.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="listing">The indicator listing.</param>
    /// <param name="quotes">The quotes to process.</param>
    /// <returns>The indicator results.</returns>
    public static IReadOnlyList<TResult> Execute<TResult>(this IndicatorListing listing, IEnumerable<IQuote> quotes)
        where TResult : class => IndicatorExecutor.Execute<IQuote, TResult>(quotes, listing);

    /// <summary>
    /// Alternative syntax: Execute an indicator from quotes using a custom indicator builder.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="quotes">The quotes to process.</param>
    /// <param name="customIndicator">The custom indicator configuration.</param>
    /// <returns>The indicator results.</returns>
    public static IReadOnlyList<TResult> Execute<TResult>(this IEnumerable<IQuote> quotes, CustomIndicatorBuilder customIndicator)
        where TResult : class => customIndicator == null
            ? throw new ArgumentNullException(nameof(customIndicator))
            : customIndicator.FromSource(quotes).Execute<TResult>();

    /// <summary>
    /// Alternative syntax: Execute an indicator from a series using a custom indicator builder.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the source series, which must implement IReusable.</typeparam>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="series">The series data to process.</param>
    /// <param name="customIndicator">The custom indicator configuration.</param>
    /// <param name="parameterName">The name of the series parameter. If null, attempts to find the first series parameter.</param>
    /// <returns>The indicator results.</returns>
    public static IReadOnlyList<TResult> Execute<TSource, TResult>(this IReadOnlyList<TSource> series, CustomIndicatorBuilder customIndicator, string? parameterName = null)
        where TSource : IReusable
        where TResult : class => customIndicator == null
            ? throw new ArgumentNullException(nameof(customIndicator))
            : customIndicator.FromSource(series, parameterName).Execute<TResult>();
}
