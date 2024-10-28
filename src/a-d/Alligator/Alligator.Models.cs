namespace Skender.Stock.Indicators;

[Serializable]
public record AlligatorResult
(
    DateTime Timestamp,
    double? Jaw,
    double? Teeth,
    double? Lips
) : ISeries;
