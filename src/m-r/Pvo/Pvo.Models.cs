namespace Skender.Stock.Indicators;

public record PvoResult
(
    DateTime Timestamp,
    double? Pvo,
    double? Signal,
    double? Histogram
) : Reusable(Timestamp, Pvo.Null2NaN());
