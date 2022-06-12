namespace Skender.Stock.Indicators;

[Serializable]
public sealed class StarcBandsResult : ResultBase
{
    public double? UpperBand { get; set; }
    public double? Centerline { get; set; }
    public double? LowerBand { get; set; }
}
