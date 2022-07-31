namespace Skender.Stock.Indicators;

// CANDLESTICK MODELS

[Serializable]
public class CandleProperties : Quote
{
    // raw sizes
    internal decimal? Size => High - Low;
    internal decimal? Body => (Open > Close) ? (Open - Close) : (Close - Open);
    internal decimal? UpperWick => High - (Open > Close ? Open : Close);
    internal decimal? LowerWick => (Open > Close ? Close : Open) - Low;

    // percent sizes
    internal double? BodyPct => (Size != 0) ? (double?)(Body / Size) : 1;
    internal double? UpperWickPct => (Size != 0) ? (double?)(UpperWick / Size) : 1;
    internal double? LowerWickPct => (Size != 0) ? (double?)(LowerWick / Size) : 1;

    // directional info
    internal bool IsBullish => Close > Open;
    internal bool IsBearish => Close < Open;
}

[Serializable]
public class CandleResult : ResultBase
{
    public CandleResult(DateTime date)
    {
        Date = date;
        Candle = new CandleProperties();
    }

    public decimal? Price { get; set; }
    public Match Match { get; set; }
    public CandleProperties Candle { get; set; }
}