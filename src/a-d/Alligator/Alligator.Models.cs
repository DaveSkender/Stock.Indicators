namespace Skender.Stock.Indicators;

public record AlligatorResult
(
    DateTime Timestamp,
    double? Jaw,
    double? Teeth,
    double? Lips
) : ISeries;
