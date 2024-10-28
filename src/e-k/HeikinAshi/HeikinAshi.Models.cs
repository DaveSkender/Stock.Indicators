namespace Skender.Stock.Indicators;

[Serializable]
public record HeikinAshiResult(
    DateTime Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume
) : Quote(Timestamp, Open, High, Low, Close, Volume);
