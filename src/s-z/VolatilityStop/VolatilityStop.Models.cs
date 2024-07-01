namespace Skender.Stock.Indicators;

public readonly record struct VolatilityStopResult
(
    DateTime Timestamp,
    double? Sar,
    bool? IsStop,

    // SAR values as long/short stop bands
    double? UpperBand,
    double? LowerBand
) : IReusable
{
    double IReusable.Value => Sar.Null2NaN();
}
