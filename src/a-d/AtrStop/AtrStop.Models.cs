namespace Skender.Stock.Indicators;

public record struct AtrStopResult : IResult
{
    public DateTime Timestamp { get; set; }
    public decimal? AtrStop { get; set; }
    public decimal? BuyStop { get; set; }
    public decimal? SellStop { get; set; }
}
