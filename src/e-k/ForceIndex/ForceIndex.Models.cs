namespace Skender.Stock.Indicators;

public record ForceIndexResult
(
    DateTime Timestamp,
    double? ForceIndex = null
) : Reusable(Timestamp, ForceIndex.Null2NaN());
