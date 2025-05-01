namespace Generators.Catalogger.Utils;

/// <summary>
/// Model for indicator information extracted from attributes.
/// </summary>
internal sealed record IndicatorInfo(
    string Uiid,
    string Name,
    string Type,
    string ContainingType,
    string MemberName,
    string Category,
    string ChartType,
    string? LegendOverride,
    List<ParameterInfo> Parameters);

/// <summary>
/// Model for parameter information extracted from attributes.
/// </summary>
internal sealed record ParameterInfo(
    string Name,
    string DisplayName,
    string DataType,
    double? DefaultValue,
    double? MinValue,
    double? MaxValue,
    Dictionary<int, string>? EnumOptions = null);
