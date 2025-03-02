[Serializable]
public record IndicatorResultConfig
{
    public string? DisplayName { get; init; }
    public string? TooltipTemplate { get; init; }
    public string? DataName { get; init; }
    public string? DataType { get; init; }
    public string? LineType { get; init; }
    public string? Stack { get; set; }
    public float LineWidth { get; set; } = 2;
    public string? DefaultColor { get; init; }
    public ChartFill? Fill { get; set; }
}
