/// <summary>
/// Represents a threshold in a chart.
/// </summary>
[Serializable]
public record class ChartThreshold
{
    public required double Value { get; init; }
    public required string Color { get; init; }
    public required string Style { get; init; }
    public ChartFill? Fill { get; set; }
}
