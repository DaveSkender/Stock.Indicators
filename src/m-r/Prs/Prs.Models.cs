namespace Skender.Stock.Indicators;

public record PrsResult
(
    DateTime Timestamp,
    double? Prs,
    double? PrsPercent
) : Reusable(Timestamp, Prs.Null2NaN());
