namespace Skender.Stock.Indicators;

[Serializable]
public sealed class RenkoResult : ResultBase, IQuote
{
    public RenkoResult(DateTime date)
    {
        Date = date;
    }

    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
    public bool IsUp { get; set; }
}
