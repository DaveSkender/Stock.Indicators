namespace Skender.Stock.Indicators;

public record WmaResult
(
    DateTime Timestamp,
    double? Wma
) : Reusable(Timestamp, Wma.Null2NaN());
