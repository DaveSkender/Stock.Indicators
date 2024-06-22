namespace Skender.Stock.Indicators;

public record struct StdDevChannelsResult : IResult
{
    public DateTime Timestamp { get; set; }
    public double? Centerline { get; set; }
    public double? UpperChannel { get; set; }
    public double? LowerChannel { get; set; }
    public bool BreakPoint { get; set; }
}
