namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a SuperTrend indicator calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="SuperTrend">The value of the SuperTrend at this point.</param>
/// <param name="UpperBand">The upper band value at this point.</param>
/// <param name="LowerBand">The lower band value at this point.</param>
[Serializable]
public record SuperTrendResult
(
    DateTime Timestamp,
    decimal? SuperTrend,
    decimal? UpperBand,
    decimal? LowerBand
) : ISeries;
