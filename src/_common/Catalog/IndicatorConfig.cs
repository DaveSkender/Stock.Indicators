namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a serializable indicator configuration that can be stored as JSON
/// and later used to build CustomIndicatorBuilder instances.
/// </summary>
public class IndicatorConfig
{
    /// <summary>
    /// The indicator ID (e.g., "EMA", "RSI", "MACD").
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The indicator style (Series, Stream, or Buffer).
    /// </summary>
    public Style Style { get; set; } = Style.Series;

    /// <summary>
    /// Parameter overrides for the indicator.
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = [];

    /// <summary>
    /// Optional display name for this configuration.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Optional description for this configuration.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Builds a CustomIndicatorBuilder from this configuration.
    /// </summary>
    /// <returns>A CustomIndicatorBuilder configured according to this config.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the indicator cannot be found.</exception>
    public CustomIndicatorBuilder ToBuilder()
    {
        IndicatorListing? listing = IndicatorRegistry.GetByIdAndStyle(Id, Style);
        return listing == null
            ? throw new InvalidOperationException($"Indicator '{Id}' with style '{Style}' not found in catalog")
            : new CustomIndicatorBuilder(listing, Parameters);
    }

    /// <summary>
    /// Creates an IndicatorConfig from a CustomIndicatorBuilder.
    /// </summary>
    /// <param name="builder">The CustomIndicatorBuilder to convert.</param>
    /// <returns>An IndicatorConfig representing the builder's configuration.</returns>
    public static IndicatorConfig FromBuilder(CustomIndicatorBuilder builder) => new() {
        Id = builder.BaseListing.Uiid,
        Style = builder.BaseListing.Style,
        Parameters = new Dictionary<string, object>(builder.ParameterOverrides),
        DisplayName = builder.BaseListing.Name
    };
}

/// <summary>
/// Extension methods for working with indicator configurations.
/// </summary>
public static class IndicatorConfigExtensions
{
    /// <summary>
    /// Converts an IndicatorConfig to a CustomIndicatorBuilder.
    /// </summary>
    /// <param name="config">The indicator configuration.</param>
    /// <returns>A CustomIndicatorBuilder.</returns>
    public static CustomIndicatorBuilder ToBuilder(this IndicatorConfig config) => config.ToBuilder();

    /// <summary>
    /// Executes an indicator configuration with the provided quotes.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="config">The indicator configuration.</param>
    /// <param name="quotes">The quotes to process.</param>
    /// <returns>The indicator results.</returns>
    public static IReadOnlyList<TResult> Execute<TResult>(this IndicatorConfig config, IEnumerable<IQuote> quotes)
        where TResult : class => config.ToBuilder().FromSource(quotes).Execute<TResult>();
}
