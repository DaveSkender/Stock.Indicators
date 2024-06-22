namespace Skender.Stock.Indicators;

public record struct RenkoResult : IQuote, IReusableResult
{
    public DateTime Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    public bool IsUp { get; set; }

    readonly double IReusableResult.Value
        => (double)Close;
}
