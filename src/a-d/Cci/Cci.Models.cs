namespace Skender.Stock.Indicators;

public record CciResult
(
    DateTime Timestamp,
    double? Cci
) : Reusable(Timestamp, Cci.Null2NaN());
