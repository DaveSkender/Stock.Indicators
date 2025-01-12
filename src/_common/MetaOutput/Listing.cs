/// <summary>
/// Represents a listing of an indicator.
/// </summary>
[Serializable]
public record class IndicatorListing
{
    public required string Name { get; init; }
    public required string Uiid { get; init; }
    public required string LegendTemplate { get; init; }
    public required string Endpoint { get; init; }
    public required string Category { get; init; }
    public required string ChartType { get; init; }
    public Order Order { get; init; } = Order.Front;

    public ChartConfig? ChartConfig { get; set; }

    public ICollection<IndicatorParamConfig>? Parameters { get; set; }
    public required ICollection<IndicatorResultConfig> Results { get; init; }
}
