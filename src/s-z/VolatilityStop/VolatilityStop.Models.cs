namespace Skender.Stock.Indicators;

[Serializable]
public record VolatilityStopResult
(
    DateTime Timestamp,
    double? Sar = null,
    bool? IsStop = null,

    // SAR values as long/short stop bands
    double? UpperBand = null,
    double? LowerBand = null

) : IReusable
{
    public double Value => Sar.Null2NaN();
}
