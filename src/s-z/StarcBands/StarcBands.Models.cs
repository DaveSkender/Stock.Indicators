namespace Skender.Stock.Indicators;

public sealed class StarcBandsResult : ResultBase
{
    public double? UpperBand { get; set; }
    public double? Centerline { get; set; }
    public double? LowerBand { get; set; }
}
