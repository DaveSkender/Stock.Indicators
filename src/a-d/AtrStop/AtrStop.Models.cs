namespace Skender.Stock.Indicators;

public sealed class AtrStopResult : ResultBase
{
    public decimal? AtrStop { get; set; }
    public decimal? BuyStop { get; set; }
    public decimal? SellStop { get; set; }
}
