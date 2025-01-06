namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Renko chart calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the Renko brick.</param>
/// <param name="Open">The opening price of the Renko brick.</param>
/// <param name="High">The highest price of the Renko brick.</param>
/// <param name="Low">The lowest price of the Renko brick.</param>
/// <param name="Close">The closing price of the Renko brick.</param>
/// <param name="Volume">The volume of the Renko brick.</param>
/// <param name="IsUp">Indicates whether the Renko brick is an up brick.</param>
/// <inheritdoc cref="Quote"/>
[Serializable]
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
