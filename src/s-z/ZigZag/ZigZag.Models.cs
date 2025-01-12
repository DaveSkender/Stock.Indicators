/// <summary>
/// Represents the result of a ZigZag indicator calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="ZigZag">The value of the ZigZag line at this point.</param>
/// <param name="PointType">The type of point (e.g., H: high or L: low).</param>
/// <param name="RetraceHigh">The value of the retrace high line at this point.</param>
/// <param name="RetraceLow">The value of the retrace low line at this point.</param>
[Serializable]
public record ZigZagResult
(
   DateTime Timestamp,
   decimal? ZigZag = null,
   string? PointType = null,
   decimal? RetraceHigh = null,
   decimal? RetraceLow = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => ZigZag.Null2NaN();
}

/// <summary>
/// Represents an evaluation point used in the ZigZag calculation.
/// </summary>
internal class ZigZagEval
{
    /// <summary>
    /// Gets or sets the index of the evaluation point.
    /// </summary>
    internal int Index { get; init; }

    /// <summary>
    /// Gets or sets the high value at the evaluation point.
    /// </summary>
    internal decimal? High { get; set; }

    /// <summary>
    /// Gets or sets the low value at the evaluation point.
    /// </summary>
    internal decimal? Low { get; set; }
}

/// <summary>
/// Represents a specific point in the ZigZag calculation.
/// </summary>
internal class ZigZagPoint
{
    /// <summary>
    /// Gets or sets the index of the point.
    /// </summary>
    internal int Index { get; set; }

    /// <summary>
    /// Gets or sets the value at the point.
    /// </summary>
    internal decimal? Value { get; set; }

    /// <summary>
    /// Gets or sets the type of the point (e.g., High or Low).
    /// </summary>
    internal string? PointType { get; set; }
}
