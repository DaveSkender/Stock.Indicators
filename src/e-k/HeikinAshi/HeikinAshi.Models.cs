namespace Skender.Stock.Indicators;

public record struct HeikinAshiResult : IReusableResult, IQuote
{
    public DateTime Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    readonly double IReusableResult.Value
        => (double)Close;
}
