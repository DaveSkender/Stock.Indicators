namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a customized indicator configuration that can be configured, sourced, and executed.
/// Supports fluent API for building indicator execution pipelines.
/// </summary>
public class ListingExecutionBuilder
{
    private readonly Dictionary<string, object> _parameterOverrides;
    private IEnumerable<IQuote>? _quotes;

    internal ListingExecutionBuilder(
        IndicatorListing baseListing,
        Dictionary<string, object>? parameterOverrides = null)
    {
        BaseListing = baseListing ?? throw new ArgumentNullException(nameof(baseListing));
        _parameterOverrides = parameterOverrides ?? [];
    }

    /// <summary>
    /// Sets or overrides a parameter value for the indicator.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to override.</param>
    /// <param name="value">The value to set for the parameter.</param>
    /// <returns>A new <see cref="ListingExecutionBuilder"/> with the parameter override applied.</returns>
    /// <exception cref="ArgumentException">Thrown when an argument is invalid</exception>
    public ListingExecutionBuilder WithParamValue(string parameterName, object value)
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

        return new ListingExecutionBuilder(BaseListing, newOverrides);
    }

    /// <summary>
    /// Sets multiple parameter values for the indicator.
    /// </summary>
    /// <param name="parameters">Dictionary of parameter names and values.</param>
    /// <returns>A new <see cref="ListingExecutionBuilder"/> with the parameter overrides applied.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <c>null</c>.</exception>
    public ListingExecutionBuilder WithParams(Dictionary<string, object> parameters)
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

        return new ListingExecutionBuilder(BaseListing, newOverrides);
    }

    /// <summary>
    /// Specifies the source quotes for the indicator calculation.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>A new <see cref="ListingExecutionBuilder"/> with the quotes set.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="quotes"/> is <c>null</c>.</exception>
    public ListingExecutionBuilder FromSource(IEnumerable<IQuote> quotes)
    {
        if (quotes == null)
        {
            throw new ArgumentNullException(nameof(quotes));
        }

        return new(BaseListing, _parameterOverrides) {
            _quotes = quotes
        };
    }

    /// <summary>
    /// Specifies a series source for the indicator calculation.
    /// This is equivalent to calling WithParamValue for a series parameter,
    /// but provides a more intuitive fluent API for chaining indicators.
    /// </summary>
    /// <typeparam name="T">The type of elements in the series, which must implement IReusable.</typeparam>
    /// <param name="series">The series data to process.</param>
    /// <param name="parameterName">The name of the series parameter. If null, attempts to find the first series parameter.</param>
    /// <returns>A new <see cref="ListingExecutionBuilder"/> with the series parameter set.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="series"/> is <c>null</c>.</exception>
    public ListingExecutionBuilder FromSource<T>(IReadOnlyList<T> series, string? parameterName = null)
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
            : ListingExecutor.Execute<TResult>(_quotes, BaseListing, _parameterOverrides);

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
                throw new ArgumentException(
                    $"Parameter '{parameterName}' expects a series (IReadOnlyList<T> where T : IReusable), but received {value.GetType().Name}", nameof(value));
            }

            // Check if the series elements implement IReusable
            Type valueType = value.GetType();

            if ((valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>))
              || valueType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReadOnlyList<>)))
            {
                Type? elementType = valueType.GetGenericArguments().FirstOrDefault()
                    ?? valueType.GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReadOnlyList<>))?.GetGenericArguments().FirstOrDefault();

                if (elementType != null && !typeof(IReusable).IsAssignableFrom(elementType))
                {
                    throw new ArgumentException(
                        $"Parameter '{parameterName}' expects series elements to implement IReusable, but received {elementType.Name}", nameof(value));
                }
            }
        }
        // Validate basic parameter types
        else if (param.DataType == "Int32" && value is not int)
        {
            throw new ArgumentException(
                $"Parameter '{parameterName}' expects an integer value, but received {value.GetType().Name}", nameof(value));
        }
        else if ((param.DataType == "Double" || param.DataType == "Nullable`1") && value is not double && value is not int && value is not null) // Allow int to double conversion and nullable
        {
            throw new ArgumentException(
                $"Parameter '{parameterName}' expects a double value, but received {value.GetType().Name}", nameof(value));
        }
        else if (param.DataType == "Boolean" && value is not bool)
        {
            throw new ArgumentException(
                $"Parameter '{parameterName}' expects a boolean value, but received {value?.GetType().Name ?? "null"}", nameof(value));
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
        IndicatorParam? seriesParam
            = BaseListing.Parameters?
                .FirstOrDefault(static p => p.DataType == "IReadOnlyList<T> where T : IReusable");

        return seriesParam?.ParameterName
            ?? throw new InvalidOperationException($"No series parameter found in indicator '{BaseListing.Uiid}'");
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
