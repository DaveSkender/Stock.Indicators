namespace Skender.Stock.Indicators;

public record MaEnvelopeResult
(
    DateTime Timestamp,
    double? Centerline,
    double? UpperEnvelope,
    double? LowerEnvelope
) : ISeries;
