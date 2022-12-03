namespace Skender.Stock.Indicators;

[Serializable]
public sealed class StarcBandsResult : ResultBase
{
    public StarcBandsResult(DateTime date)
    {
        Date = date;
    }

    public double? UpperBand { get; set; }
    public double? Centerline { get; set; }
    public double? LowerBand { get; set; }
}
