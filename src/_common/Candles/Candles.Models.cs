namespace Skender.Stock.Indicators;

// CANDLESTICK MODELS

public record struct CandleProperties : IQuote, IReusable
{
    // base quote properties
    public DateTime Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    readonly double IReusable.Value
        => (double)Close;

    // raw sizes
    public readonly decimal? Size => High - Low;
    public readonly decimal? Body => (Open > Close) ? (Open - Close) : (Close - Open);
    public readonly decimal? UpperWick => High - (Open > Close ? Open : Close);
    public readonly decimal? LowerWick => (Open > Close ? Close : Open) - Low;

    // percent sizes
    public readonly double? BodyPct => (Size != 0) ? (double?)(Body / Size) : 1;
    public readonly double? UpperWickPct => (Size != 0) ? (double?)(UpperWick / Size) : 1;
    public readonly double? LowerWickPct => (Size != 0) ? (double?)(LowerWick / Size) : 1;

    // directional info
    public readonly bool IsBullish => Close > Open;
    public readonly bool IsBearish => Close < Open;

    // this is only an appropriate
    // implementation for record types
    public readonly bool Equals(IQuote? other)
      => base.Equals(other);
}

public record struct CandleResult : IResult
{
    public DateTime Timestamp { get; set; }
    public decimal? Price { get; set; }
    public Match Match { get; set; }
    public CandleProperties Candle { get; set; }
}
