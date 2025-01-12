/// <summary>
/// Represents a fill in a chart.
/// </summary>
[Serializable]
public record class ChartFill
{
    public required string Target { get; init; }
    public required string ColorAbove { get; init; }
    public required string ColorBelow { get; init; }
}

