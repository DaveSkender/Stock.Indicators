namespace Skender.Stock.Indicators;

public record VolatilityStopResult
(
    DateTime Timestamp,
    double? Sar = null,
    bool? IsStop = null,

    // SAR values as long/short stop bands
    double? UpperBand = null,
    double? LowerBand = null
) : Reusable(Timestamp, Sar.Null2NaN());
