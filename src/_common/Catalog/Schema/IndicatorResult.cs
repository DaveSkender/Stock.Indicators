namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the configuration for an indicator result.
/// </summary>
[Serializable]
public record IndicatorResult
{
    /// <summary>
    /// Gets or sets the display name of the result.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Gets or sets the data name of the result.
    /// </summary>
    public required string DataName { get; init; }

    /// <summary>
    /// Gets or sets the data type of the result.
    /// </summary>
    public required ResultType DataType { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this result is the default output.
    /// </summary>
    public bool IsDefault { get; init; }
}
