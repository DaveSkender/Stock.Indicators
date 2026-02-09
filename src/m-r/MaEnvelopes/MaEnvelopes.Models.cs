namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Moving Average Envelope calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Centerline">The value of the centerline (moving average).</param>
/// <param name="UpperEnvelope">The value of the upper envelope.</param>
/// <param name="LowerEnvelope">The value of the lower envelope.</param>
[Serializable]
public record MaEnvelopeResult
(
    DateTime Timestamp,
    double? Centerline,
    double? UpperEnvelope,
    double? LowerEnvelope
) : ISeries;
