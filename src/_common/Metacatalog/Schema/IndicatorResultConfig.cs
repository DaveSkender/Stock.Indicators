/// <summary>
/// Represents the configuration for an indicator result.
/// </summary>
[Serializable]
public record IndicatorResultConfig
{
    /// <summary>
    /// Gets or sets the display name of the result.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Gets or sets the tooltip template for the result.
    /// </summary>
    public string? TooltipTemplate { get; init; }

    /// <summary>
    /// Gets or sets the data name of the result.
    /// </summary>
    public string? DataName { get; init; }

    /// <summary>
    /// Gets or sets the data type of the result.
    /// </summary>
    public string? DataType { get; init; }

    /// <summary>
    /// Gets or sets the line type of the result.
    /// </summary>
    public string? LineType { get; init; }

    /// <summary>
    /// Gets or sets the stack name for the result.
    /// </summary>
    public string? Stack { get; init; }

    /// <summary>
    /// Gets or sets the line width of the result.
    /// </summary>
    public float LineWidth { get; init; } = 2;

    /// <summary>
    /// Gets or sets the default color of the result.
    /// </summary>
    public string? DefaultColor { get; init; }

    /// <summary>
    /// Gets or sets the fill configuration for the result.
    /// </summary>
    public ChartFill? Fill { get; init; }
}
