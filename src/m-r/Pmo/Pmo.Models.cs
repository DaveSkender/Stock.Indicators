namespace Skender.Stock.Indicators;

public record PmoResult
(
    DateTime Timestamp,
    double? Pmo,
    double? Signal
) : Reusable(Timestamp, Pmo.Null2NaN());
