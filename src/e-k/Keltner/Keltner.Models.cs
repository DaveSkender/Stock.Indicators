namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Keltner Channel calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="UpperBand">The upper band value of the Keltner Channel.</param>
/// <param name="Centerline">The centerline value of the Keltner Channel.</param>
/// <param name="LowerBand">The lower band value of the Keltner Channel.</param>
/// <param name="Width">The width of the Keltner Channel.</param>
/// <param name="Atr">The Average True Range value (interim data for streaming).</param>
[Serializable]
public record KeltnerResult
(
    DateTime Timestamp,
    double? UpperBand = null,
    double? Centerline = null,
    double? LowerBand = null,
    double? Width = null,

    // extra/interim data
    double? Atr = null
) : ISeries;
