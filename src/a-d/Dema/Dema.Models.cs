namespace Skender.Stock.Indicators;

public record DemaResult
(
    DateTime Timestamp,
    double? Dema = null
) : Reusable(Timestamp, Dema.Null2NaN());
