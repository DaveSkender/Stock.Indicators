namespace Skender.Stock.Indicators;

public record CmoResult
(
    DateTime Timestamp,
    double? Cmo = null
) : Reusable(Timestamp, Cmo.Null2NaN());
