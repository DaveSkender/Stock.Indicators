namespace Skender.Stock.Indicators;

public record struct VortexResult : IResult
{
    public DateTime Timestamp { get; set; }
    public double? Pvi { get; set; }
    public double? Nvi { get; set; }
}
