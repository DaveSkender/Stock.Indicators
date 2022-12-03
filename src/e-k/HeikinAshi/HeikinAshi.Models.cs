namespace Skender.Stock.Indicators;

[Serializable]
public sealed class HeikinAshiResult : ResultBase, IQuote
{
    public HeikinAshiResult(DateTime date)
    {
        Date = date;
    }

    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
}
