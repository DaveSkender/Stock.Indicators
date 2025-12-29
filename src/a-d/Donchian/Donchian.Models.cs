namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Donchian Channel calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="UpperBand">The upper band value of the Donchian Channel.</param>
/// <param name="Centerline">The centerline value of the Donchian Channel.</param>
/// <param name="LowerBand">The lower band value of the Donchian Channel.</param>
/// <param name="Width">The width of the Donchian Channel.</param>
[Serializable]
public record DonchianResult
(
    DateTime Timestamp,
    decimal? UpperBand = null,
    decimal? Centerline = null,
    decimal? LowerBand = null,
    decimal? Width = null
) : ISeries;
