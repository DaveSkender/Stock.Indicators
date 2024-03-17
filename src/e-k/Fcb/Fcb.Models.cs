namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class FcbResult : IResult
{
    public DateTime Timestamp { get; set; }
    public decimal? UpperBand { get; set; }
    public decimal? LowerBand { get; set; }
}
