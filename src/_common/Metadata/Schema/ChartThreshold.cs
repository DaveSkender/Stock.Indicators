/// <summary>
/// Represents a threshold configuration for a chart.
/// </summary>
[Serializable]
public record ChartThreshold
{
    /// <summary>
    /// Gets or sets the static y-axis position value of the threshold.
    /// </summary>
    public required double Value { get; init; }

    /// <summary>
    /// Gets or sets the color of the threshold line.
    /// </summary>
    public required string Color { get; init; }

    /// <summary>
    /// Gets or sets the style of the threshold line.
    /// </summary>
    public required string Style { get; init; }

    /// <summary>
    /// Gets or sets the fill configuration for the threshold.
    /// </summary>
    public ChartFill? Fill { get; init; }
}
