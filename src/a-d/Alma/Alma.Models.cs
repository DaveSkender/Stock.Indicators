namespace Skender.Stock.Indicators;

public record AlmaResult
(
    DateTime Timestamp,
    double? Alma
) : Reusable(Timestamp, Alma.Null2NaN());
