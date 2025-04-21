namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the configuration for an indicator parameter.
/// </summary>
[Serializable]
public record IndicatorParamConfig
{
    /// <summary>
    /// Gets or sets the display name of the parameter.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Gets or sets the name of the parameter.
    /// </summary>
    public string? ParamName { get; init; }

    /// <summary>
    /// Gets or sets the data type of the parameter.
    /// </summary>
    public string? DataType { get; init; }

    /// <summary>
    /// Gets or sets the minimum value for the parameter.
    /// </summary>
    public double Minimum { get; init; } // greater than

    /// <summary>
    /// Gets or sets the maximum value for the parameter.
    /// </summary>
    public double Maximum { get; init; } // less than

    /// <summary>
    /// Gets or sets the default value for the parameter.
    /// </summary>
    public double? DefaultValue { get; init; }
}
