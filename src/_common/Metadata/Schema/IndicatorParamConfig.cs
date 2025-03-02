[Serializable]
public record IndicatorParamConfig
{
    public string? DisplayName { get; init; }
    public string? ParamName { get; init; }
    public string? DataType { get; init; }
    public double Minimum { get; init; } // greater than
    public double Maximum { get; init; } // less than
    public double? DefaultValue { get; init; }
}
