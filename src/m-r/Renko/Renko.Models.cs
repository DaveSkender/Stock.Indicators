namespace Skender.Stock.Indicators;

/// <inheritdoc cref="Quote"/>
public record RenkoResult
(
    DateTime Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume,
    bool IsUp
) : Quote(Timestamp, Open, High, Low, Close, Volume);
