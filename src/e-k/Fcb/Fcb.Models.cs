namespace Skender.Stock.Indicators;

public record struct FcbResult : IResult
{
    public DateTime Timestamp { get; set; }
    public decimal? UpperBand { get; set; }
    public decimal? LowerBand { get; set; }
}
