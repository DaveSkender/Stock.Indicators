/// <summary>
/// Represents the configuration of an indicator result.
/// </summary>
[Serializable]
public record class IndicatorResultConfig
{
    public required string DisplayName { get; init; }
    public required string TooltipTemplate { get; init; }
    public required string DataName { get; init; }
    public required string DataType { get; init; }
    public required string LineType { get; init; }
    public string? Stack { get; set; }
    public float LineWidth { get; set; } = 2;
    public required string DefaultColor { get; init; }
    public ChartFill? Fill { get; set; }

}
