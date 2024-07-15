namespace Skender.Stock.Indicators;

public record BopResult
(
    DateTime Timestamp,
    double? Bop
) : Reusable(Timestamp, Bop.Null2NaN());
