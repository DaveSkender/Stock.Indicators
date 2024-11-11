namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Heikin-Ashi calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Open">The open price of the Heikin-Ashi candle.</param>
/// <param name="High">The high price of the Heikin-Ashi candle.</param>
/// <param name="Low">The low price of the Heikin-Ashi candle.</param>
/// <param name="Close">The close price of the Heikin-Ashi candle.</param>
/// <param name="Volume">The volume of the Heikin-Ashi candle.</param>
[Serializable]
public record HeikinAshiResult(
    DateTime Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume
) : Quote(Timestamp, Open, High, Low, Close, Volume);
