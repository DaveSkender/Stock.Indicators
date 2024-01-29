namespace Skender.Stock.Indicators;

public sealed record class VortexResult : IResult
{
    public DateTime TickDate { get; set; }
    public double? Pvi { get; set; }
    public double? Nvi { get; set; }
}
