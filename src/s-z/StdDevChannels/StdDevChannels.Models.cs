namespace Skender.Stock.Indicators;

[Serializable]
public sealed class StdDevChannelsResult : ResultBase
{
    public StdDevChannelsResult(DateTime date)
    {
        Date = date;
    }

    public double? Centerline { get; set; }
    public double? UpperChannel { get; set; }
    public double? LowerChannel { get; set; }
    public bool BreakPoint { get; set; }
}
