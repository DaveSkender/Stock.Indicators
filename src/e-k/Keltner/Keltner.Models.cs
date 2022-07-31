namespace Skender.Stock.Indicators;

[Serializable]
public sealed class KeltnerResult : ResultBase
{
    public KeltnerResult(DateTime date)
    {
        Date = date;
    }

    public double? UpperBand { get; set; }
    public double? Centerline { get; set; }
    public double? LowerBand { get; set; }
    public double? Width { get; set; }
}
