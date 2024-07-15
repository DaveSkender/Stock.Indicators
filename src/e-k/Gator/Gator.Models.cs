namespace Skender.Stock.Indicators;

public record GatorResult
(
    DateTime Timestamp,
    double? Upper = null,
    double? Lower = null,
    bool? UpperIsExpanding = null,
    bool? LowerIsExpanding = null
) : ISeries;
