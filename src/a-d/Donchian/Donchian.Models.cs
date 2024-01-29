namespace Skender.Stock.Indicators;

public sealed record class DonchianResult : IResult
{
    public DateTime TickDate { get; set; }
    public decimal? UpperBand { get; set; }
    public decimal? Centerline { get; set; }
    public decimal? LowerBand { get; set; }
    public decimal? Width { get; set; }
}
