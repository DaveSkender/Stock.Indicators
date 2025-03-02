namespace Skender.Stock.Indicators;

/// <summary>
/// Represents an indicator listing with its configuration and parameters.
/// </summary>
[Serializable]
public record IndicatorListing
{
    /// <summary>
    /// Gets or sets the name of the indicator.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the unique identifier of the indicator.
    /// </summary>
    public required string Uiid { get; init; }

    /// <summary>
    /// Gets or sets the legend template for the indicator.
    /// </summary>
    public required string LegendTemplate { get; init; }

    /// <summary>
    /// Gets or sets the endpoint URL for the indicator.
    /// </summary>
    public required string Endpoint { get; init; }

    /// <summary>
    /// Gets or sets the category of the indicator.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Gets or sets the chart type for the indicator.
    /// </summary>
    public required string ChartType { get; init; }

    /// <summary>
    /// Gets or sets the order in which the indicator is rendered on the chart.
    /// </summary>
    public Order Order { get; init; } = Order.Front;

    /// <summary>
    /// Gets or sets the chart configuration for the indicator.
    /// </summary>
    public ChartConfig? ChartConfig { get; init; }

    /// <summary>
    /// Gets or sets the collection of parameters for the indicator.
    /// </summary>
    public ICollection<IndicatorParamConfig>? Parameters { get; init; }

    /// <summary>
    /// Gets or sets the collection of result configurations for the indicator.
    /// </summary>
    public required ICollection<IndicatorResultConfig> Results { get; init; }
}
