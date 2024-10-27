namespace Skender.Stock.Indicators;

[Serializable]
public record StdDevChannelsResult
(
    DateTime Timestamp,
    double? Centerline = null,
    double? UpperChannel = null,
    double? LowerChannel = null,
    bool BreakPoint = false
) : ISeries;
