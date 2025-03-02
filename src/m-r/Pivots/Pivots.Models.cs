namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a pivot points calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the pivot point.</param>
/// <param name="HighPoint">The high point value.</param>
/// <param name="LowPoint">The low point value.</param>
/// <param name="HighLine">The high line value.</param>
/// <param name="LowLine">The low line value.</param>
/// <param name="HighTrend">The high trend direction.</param>
/// <param name="LowTrend">The low trend direction.</param>
[Serializable]
public record PivotsResult
(
   DateTime Timestamp,
   decimal? HighPoint,
   decimal? LowPoint,
   decimal? HighLine,
   decimal? LowLine,
   PivotTrend? HighTrend,
   PivotTrend? LowTrend
) : ISeries;

/// <summary>
/// Represents the trend direction of a pivot point.
/// </summary>
public enum PivotTrend
{
    /// <summary>
    /// Higher high trend.
    /// </summary>
    Hh,

    /// <summary>
    /// Lower high trend.
    /// </summary>
    Lh,

    /// <summary>
    /// Higher low trend.
    /// </summary>
    Hl,

    /// <summary>
    /// Lower low trend.
    /// </summary>
    Ll
}
