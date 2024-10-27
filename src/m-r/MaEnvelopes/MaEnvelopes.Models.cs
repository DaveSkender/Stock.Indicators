namespace Skender.Stock.Indicators;

[Serializable]
public record MaEnvelopeResult
(
    DateTime Timestamp,
    double? Centerline,
    double? UpperEnvelope,
    double? LowerEnvelope
) : ISeries;
