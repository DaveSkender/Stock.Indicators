namespace Skender.Stock.Indicators;

[Serializable]
public class AtrStopResult : ResultBase
{
    public AtrStopResult(DateTime date)
    {
        Date = date;
    }

    public decimal? AtrStop { get; set; }
    public decimal? BuyStop { get; set; }
    public decimal? SellStop { get; set; }
}
