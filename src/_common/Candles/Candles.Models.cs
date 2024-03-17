namespace Skender.Stock.Indicators;

// CANDLESTICK MODELS

public record CandleProperties : Quote
{
    // raw sizes
    public decimal? Size => High - Low;
    public decimal? Body => (Open > Close) ? (Open - Close) : (Close - Open);
    public decimal? UpperWick => High - (Open > Close ? Open : Close);
    public decimal? LowerWick => (Open > Close ? Close : Open) - Low;

    // percent sizes
    public double? BodyPct => (Size != 0) ? (double?)(Body / Size) : 1;
    public double? UpperWickPct => (Size != 0) ? (double?)(UpperWick / Size) : 1;
    public double? LowerWickPct => (Size != 0) ? (double?)(LowerWick / Size) : 1;

    // directional info
    public bool IsBullish => Close > Open;
    public bool IsBearish => Close < Open;
}

public record class CandleResult : IResult
{
    public CandleResult(DateTime date, Match match)
    {
        Timestamp = date;
        Match = match;
    }

    public DateTime Timestamp { get; private set; }
    public decimal? Price { get; set; }
    public Match Match { get; set; }
    public CandleProperties Candle { get; set; } = new CandleProperties();
}
