namespace Skender.Stock.Indicators;

public record struct SuperTrendResult : IResult
{
    public DateTime Timestamp { get; set; }
    public decimal? SuperTrend { get; set; }
    public decimal? UpperBand { get; set; }
    public decimal? LowerBand { get; set; }
}
