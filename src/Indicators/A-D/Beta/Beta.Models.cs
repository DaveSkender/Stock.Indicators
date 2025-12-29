namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Beta calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Beta">The Beta value.</param>
/// <param name="BetaUp">The Beta+ upside value.</param>
/// <param name="BetaDown">The Beta- downside value.</param>
/// <param name="Ratio">The ratio of BetaUp to BetaDown.</param>
/// <param name="Convexity">The convexity of the Beta.</param>
/// <param name="ReturnsEval">The returns of the evaluated asset.</param>
/// <param name="ReturnsMrkt">The returns of the market.</param>
[Serializable]
public record BetaResult(
    DateTime Timestamp,
    double? Beta = null,
    double? BetaUp = null,
    double? BetaDown = null,
    double? Ratio = null,
    double? Convexity = null,
    double? ReturnsEval = null,
    double? ReturnsMrkt = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Beta.Null2NaN();
}

/// <summary>
/// Specifies the type of Beta calculation.
/// </summary>
public enum BetaType
{
    /// <summary>
    /// Standard Beta only
    /// </summary>
    Standard = 0,

    /// <summary>
    /// Beta+ updside only
    /// </summary>
    Up = 1,

    /// <summary>
    /// Beta- downside only
    /// </summary>
    Down = 2,

    /// <summary>
    /// Calculation all Beta types
    /// </summary>
    All = 3
}
