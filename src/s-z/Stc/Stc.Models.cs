namespace Skender.Stock.Indicators;

public record StcResult
(
    DateTime Timestamp,
    double? Stc
) : Reusable(Timestamp, Stc.Null2NaN());
