/// <summary>
/// Represents the fill configuration for a chart threshold.
/// </summary>
[Serializable]
public record ChartFill
{
    /// <summary>
    /// Gets or sets the target for the fill between threshold.
    /// </summary>
    public required string Target { get; init; }

    /// <summary>
    /// Gets or sets the color above the threshold.
    /// </summary>
    public required string ColorAbove { get; init; }

    /// <summary>
    /// Gets or sets the color below the threshold.
    /// </summary>
    public required string ColorBelow { get; init; }
}
