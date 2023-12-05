namespace Skender.Stock.Indicators;

public sealed class HeikinAshiResult : ResultBase, IQuote
{
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
}
