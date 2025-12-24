namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Fisher Transform calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Fisher">The Fisher Transform value.</param>
/// <param name="Trigger">The trigger value.</param>
[Serializable]
public record FisherTransformResult
(
    DateTime Timestamp,
    double Fisher,
    double? Trigger
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Fisher;
}
