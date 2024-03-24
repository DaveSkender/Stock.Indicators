namespace Skender.Stock.Indicators;

public sealed record class RenkoResult : IResult, IQuote
{
    public DateTime Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    public bool IsUp { get; set; }

    public bool Equals(IQuote? other)
    => base.Equals(other);
}
