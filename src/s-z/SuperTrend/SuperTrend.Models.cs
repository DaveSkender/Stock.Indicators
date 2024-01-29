namespace Skender.Stock.Indicators;

public sealed record class SuperTrendResult : IResult
{
    public DateTime TickDate { get; set; }
    public decimal? SuperTrend { get; set; }
    public decimal? UpperBand { get; set; }
    public decimal? LowerBand { get; set; }
}
