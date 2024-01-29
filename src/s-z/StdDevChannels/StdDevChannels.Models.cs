namespace Skender.Stock.Indicators;

public sealed record class StdDevChannelsResult : IResult
{
    public DateTime TickDate { get; set; }
    public double? Centerline { get; set; }
    public double? UpperChannel { get; set; }
    public double? LowerChannel { get; set; }
    public bool BreakPoint { get; set; }
}
