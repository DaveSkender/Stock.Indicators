namespace Skender.Stock.Indicators;

public record MfiResult
(
    DateTime Timestamp,
    double? Mfi
) : Reusable(Timestamp, Mfi.Null2NaN());
