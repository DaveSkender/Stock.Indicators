namespace Skender.Stock.Indicators;

[Serializable]
public record GatorResult
(
    DateTime Timestamp,
    double? Upper = null,
    double? Lower = null,
    bool? UpperIsExpanding = null,
    bool? LowerIsExpanding = null
) : ISeries;
