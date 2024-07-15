namespace Skender.Stock.Indicators;

public record FisherTransformResult
(
    DateTime Timestamp,
    double? Fisher,
    double? Trigger
) : Reusable(Timestamp, Fisher.Null2NaN());
