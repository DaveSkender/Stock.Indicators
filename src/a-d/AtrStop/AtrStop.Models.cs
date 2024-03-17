namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class AtrStopResult : IResult
{
    public DateTime Timestamp { get; set; }
    public decimal? AtrStop { get; set; }
    public decimal? BuyStop { get; set; }
    public decimal? SellStop { get; set; }
}
