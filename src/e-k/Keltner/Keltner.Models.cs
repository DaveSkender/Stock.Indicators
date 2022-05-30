namespace Skender.Stock.Indicators;

[Serializable]
public sealed class KeltnerResult : ResultBase
{
    public double? UpperBand { get; set; }
    public double? Centerline { get; set; }
    public double? LowerBand { get; set; }
    public double? Width { get; set; }
}
