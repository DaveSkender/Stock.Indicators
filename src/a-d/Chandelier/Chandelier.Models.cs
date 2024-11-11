namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Chandelier Exit calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="ChandelierExit">The Chandelier Exit value.</param>
[Serializable]
public record ChandelierResult
(
    DateTime Timestamp,
    double? ChandelierExit
) : IReusable
{
    /// <inheritdoc/>
    public double Value => ChandelierExit.Null2NaN();
}

/// <summary>
/// Specifies the type of Chandelier Exit.
/// </summary>
public enum ChandelierType
{
    /// <summary>
    /// Represents a long Chandelier Exit.
    /// </summary>
    Long = 0,

    /// <summary>
    /// Represents a short Chandelier Exit.
    /// </summary>
    Short = 1
}
