using System.Globalization;

namespace Skender.Stock.Indicators;

/// <summary>
/// Builder for creating IndicatorListing instances using a fluent interface.
/// </summary>
public class IndicatorListingBuilder
{
    private string _name = string.Empty;
    private string _id = string.Empty;
    private Category _category = Category.Undefined;
    private string? _methodName;
    private readonly List<IndicatorParam> _parameters = [];
    private readonly List<IndicatorResult> _results = [];

    /// <summary>
    /// Gets the current base URL setting for the builder.
    /// </summary>
    protected Uri? CurrentBaseUrl { get; private set; }

    /// <summary>
    /// Gets the current style setting for the builder.
    /// </summary>
    protected Style CurrentStyle { get; private set; } = Style.Series;

    /// <summary>
    /// Sets the name of the indicator.
    /// </summary>
    /// <param name="name">The indicator name.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public IndicatorListingBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the ID of the indicator.
    /// </summary>
    /// <param name="id">The indicator ID.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public IndicatorListingBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    /// <summary>
    /// Sets the style of the indicator.
    /// </summary>
    /// <param name="style">The indicator style.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public IndicatorListingBuilder WithStyle(Style style)
    {
        CurrentStyle = style;
        return this;
    }

    /// <summary>
    /// Sets the category of the indicator.
    /// </summary>
    /// <param name="category">The indicator category.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public IndicatorListingBuilder WithCategory(Category category)
    {
        _category = category;
        return this;
    }

    /// <summary>
    /// Sets the method name for automation use cases.
    /// </summary>
    /// <param name="methodName">The method name.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public IndicatorListingBuilder WithMethodName(string methodName)
    {
        _methodName = methodName;
        return this;
    }

    /// <summary>
    /// Sets the base URL of the indicator.
    /// </summary>
    /// <param name="baseUrl">The base URL as a Uri.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public IndicatorListingBuilder WithBaseUrl(Uri baseUrl)
    {
        CurrentBaseUrl = baseUrl;
        return this;
    }

    /// <summary>
    /// Adds a parameter to the indicator.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="displayName">The display name of the parameter.</param>
    /// <param name="description">Optional description of the parameter.</param>
    /// <param name="isRequired">Whether the parameter is required.</param>
    /// <param name="defaultValue">Optional default value for the parameter.</param>
    /// <param name="minimum">Optional minimum value for the parameter.</param>
    /// <param name="maximum">Optional maximum value for the parameter.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public IndicatorListingBuilder AddParameter<T>(
        string parameterName,
        string displayName,
        string? description = null,
        bool isRequired = false,
        object? defaultValue = null,
        double? minimum = null,
        double? maximum = null)
    {
        _parameters.Add(new IndicatorParam {
            ParameterName = parameterName,
            DisplayName = displayName,
            Description = description,
            DataType = typeof(T).Name,
            IsRequired = isRequired,
            DefaultValue = defaultValue,
            Minimum = minimum,
            Maximum = maximum
        });

        return this;
    }

    /// <summary>
    /// Adds an enum parameter to the indicator.
    /// </summary>
    /// <typeparam name="T">The enum type of the parameter.</typeparam>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="displayName">The display name of the parameter.</param>
    /// <param name="description">Optional description of the parameter.</param>
    /// <param name="isRequired">Whether the parameter is required.</param>
    /// <param name="defaultValue">Optional default value for the parameter.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public IndicatorListingBuilder AddEnumParameter<T>(
        string parameterName,
        string displayName,
        string? description = null,
        bool isRequired = false,
        T? defaultValue = default)
        where T : struct, Enum
    {
        // Create enum options dictionary
        Dictionary<int, string> enumOptions = [];
        foreach (T enumValue in Enum.GetValues<T>())
        {
            enumOptions.Add(Convert.ToInt32(enumValue, CultureInfo.InvariantCulture), enumValue.ToString());
        }

        _parameters.Add(new IndicatorParam {
            ParameterName = parameterName,
            DisplayName = displayName,
            Description = description,
            DataType = "enum",
            IsRequired = isRequired,
            DefaultValue = defaultValue.HasValue ? Convert.ToInt32(defaultValue.Value, CultureInfo.InvariantCulture) : null,
            Minimum = null,
            Maximum = null,
            EnumOptions = enumOptions
        });

        return this;
    }

    /// <summary>
    /// Adds a date parameter to the indicator.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="displayName">The display name of the parameter.</param>
    /// <param name="description">Optional description of the parameter.</param>
    /// <param name="isRequired">Whether the parameter is required.</param>
    /// <param name="defaultValue">Optional default value for the parameter.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public IndicatorListingBuilder AddDateParameter(
        string parameterName,
        string displayName,
        string? description = null,
        bool isRequired = false,
        DateTime? defaultValue = null)
    {
        _parameters.Add(new IndicatorParam {
            ParameterName = parameterName,
            DisplayName = displayName,
            Description = description,
            DataType = "DateTime",
            IsRequired = isRequired,
            DefaultValue = defaultValue?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Minimum = null,
            Maximum = null
        });

        return this;
    }

    /// <summary>
    /// Adds a series parameter for IReadOnlyList&lt;T&gt; where T : IReusable type parameters.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="displayName">The display name of the parameter.</param>
    /// <param name="description">Optional description of the parameter.</param>
    /// <param name="isRequired">Whether the parameter is required.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public IndicatorListingBuilder AddSeriesParameter(
        string parameterName,
        string displayName,
        string? description = null,
        bool isRequired = true)
    {
        _parameters.Add(new IndicatorParam {
            ParameterName = parameterName,
            DisplayName = displayName,
            Description = description,
            DataType = "IReadOnlyList<T> where T : IReusable",
            IsRequired = isRequired,
            DefaultValue = null,
            Minimum = null,
            Maximum = null
        });

        return this;
    }

    /// <summary>
    /// Adds a result to the indicator.
    /// </summary>
    /// <param name="dataName">The name of the data property.</param>
    /// <param name="displayName">The display name of the result.</param>
    /// <param name="dataType">The type of the result.</param>
    /// <param name="isDefault">Whether this is the default result.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public IndicatorListingBuilder AddResult(
        string dataName,
        string displayName,
        ResultType dataType = ResultType.Default,
        bool isDefault = false)
    {
        _results.Add(new IndicatorResult {
            DataName = dataName,
            DisplayName = displayName,
            DataType = dataType,
            IsDefault = isDefault
        });

        return this;
    }

    /// <summary>
    /// Adds common price High, Low, Close results to the indicator.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public IndicatorListingBuilder AddPriceHlcResult()
    {
        AddResult("High", "High", ResultType.Default, false);
        AddResult("Low", "Low", ResultType.Default, false);
        AddResult("Close", "Close", ResultType.Default, false);
        return this;
    }

    /// <summary>
    /// Builds the IndicatorListing instance.
    /// </summary>
    /// <returns>A new IndicatorListing instance.</returns>
    public IndicatorListing Build()
    {
        ValidateBeforeBuild();
        return new IndicatorListing(CurrentBaseUrl?.ToString() ?? string.Empty) {
            Name = _name,
            Uiid = _id,
            Style = CurrentStyle,
            Category = _category,
            Parameters = _parameters.Count > 0 ? _parameters : null,
            Results = _results,
            MethodName = _methodName,
            LegendTemplate = GenerateLegendTemplate()
        };
    }

    private string GenerateLegendTemplate() =>
        // A simple legend template based on the indicator name
        $"{_name} ({_id})";

    private void ValidateBeforeBuild()
    {
        if (string.IsNullOrWhiteSpace(_name))
        {
            throw new InvalidOperationException("Indicator name must be specified.");
        }

        if (string.IsNullOrWhiteSpace(_id))
        {
            throw new InvalidOperationException("Indicator ID must be specified.");
        }

        if (_results.Count == 0)
        {
            throw new InvalidOperationException("At least one result must be specified.");
        }

        // Check for duplicate parameter names
        List<string> parameterNames = _parameters.Select(p => p.ParameterName).ToList();
        if (parameterNames.Count != parameterNames.Distinct().Count())
        {
            throw new InvalidOperationException("Duplicate parameter names are not allowed.");
        }

        // Check for duplicate result names
        List<string> resultNames = _results.Select(r => r.DataName).ToList();
        if (resultNames.Count != resultNames.Distinct().Count())
        {
            throw new InvalidOperationException("Duplicate result names are not allowed.");
        }

        // Ensure at least one default result if there are multiple results
        if (_results.Count > 1 && !_results.Any(r => r.IsDefault))
        {
            throw new InvalidOperationException("At least one result must be marked as default when there are multiple results.");
        }
    }
}
