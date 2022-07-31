namespace Skender.Stock.Indicators;

[Serializable]
public sealed class VolatilityStopResult : ResultBase, IReusableResult
{
    public VolatilityStopResult(DateTime date)
    {
        Date = date;
    }

    public double? Sar { get; set; }
    public bool? IsStop { get; set; }

    // SAR values as long/short stop bands
    public double? UpperBand { get; set; }
    public double? LowerBand { get; set; }

    double? IReusableResult.Value => Sar;
}
