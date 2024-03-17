namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class VolatilityStopResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Sar { get; set; }
    public bool? IsStop { get; set; }

    // SAR values as long/short stop bands
    public double? UpperBand { get; set; }
    public double? LowerBand { get; set; }

    double IReusableResult.Value => Sar.Null2NaN();
}
