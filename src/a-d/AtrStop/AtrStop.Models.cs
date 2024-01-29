namespace Skender.Stock.Indicators;

public sealed record class AtrStopResult : IResult
{
    public DateTime TickDate { get; set; }
    public decimal? AtrStop { get; set; }
    public decimal? BuyStop { get; set; }
    public decimal? SellStop { get; set; }
}
