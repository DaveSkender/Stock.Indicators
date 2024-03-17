namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class DonchianResult : IResult
{
    public DateTime Timestamp { get; set; }
    public decimal? UpperBand { get; set; }
    public decimal? Centerline { get; set; }
    public decimal? LowerBand { get; set; }
    public decimal? Width { get; set; }
}
