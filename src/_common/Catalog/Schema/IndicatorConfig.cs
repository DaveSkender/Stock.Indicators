namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a serializable indicator configuration that can be stored as JSON
/// and later used to build <see cref="ListingExecutionBuilder"/> instances.
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
    public Dictionary<string, object> Parameters { get; init; } = [];

    /// <summary>
    /// Optional display name for this configuration.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Optional description for this configuration.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Builds a <see cref="ListingExecutionBuilder"/> from this configuration.
    /// </summary>
    /// <returns>A <see cref="ListingExecutionBuilder"/> configured according to this config.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the indicator cannot be found.</exception>
    public ListingExecutionBuilder ToBuilder()
    {
        IndicatorListing? listing = Catalog.Get(Id, Style);
        return listing == null
            ? throw new InvalidOperationException($"Indicator '{Id}' with style '{Style}' not found in catalog")
            : new ListingExecutionBuilder(listing, Parameters);
    }

    /// <summary>
    /// Creates an <see cref="IndicatorConfig"/> from a <see cref="ListingExecutionBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ListingExecutionBuilder"/> to convert.</param>
    /// <returns>An <see cref="IndicatorConfig"/> representing the builder's configuration.</returns>
    /// <exception cref="ArgumentNullException">Thrown when builder is null.</exception>
    public static IndicatorConfig FromBuilder(ListingExecutionBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return new() {
            Id = builder.BaseListing.Uiid,
            Style = builder.BaseListing.Style,
            Parameters = new Dictionary<string, object>(builder.ParameterOverrides),
            DisplayName = builder.BaseListing.Name
        };
    }
}

/// <summary>
/// Extension methods for working with indicator configurations.
/// </summary>
public static class IndicatorConfigExtensions
{
    /// <summary>
    /// Converts an <see cref="IndicatorConfig"/> to a <see cref="ListingExecutionBuilder"/>.
    /// </summary>
    /// <param name="config">The indicator configuration.</param>
    /// <returns>A <see cref="ListingExecutionBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when config is null.</exception>
    public static ListingExecutionBuilder ToBuilder(this IndicatorConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        return config.ToBuilder();
    }

    /// <summary>
    /// Executes an indicator configuration with the provided quotes.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="config">The indicator configuration.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>The indicator results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when config is null.</exception>
    public static IReadOnlyList<TResult> Execute<TResult>(this IndicatorConfig config, IEnumerable<IQuote> quotes)
        where TResult : class
    {
        ArgumentNullException.ThrowIfNull(config);
        return config.ToBuilder().FromSource(quotes).Execute<TResult>();
    }
}
