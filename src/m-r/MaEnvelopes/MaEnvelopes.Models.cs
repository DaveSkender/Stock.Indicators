namespace Skender.Stock.Indicators;

public readonly record struct MaEnvelopeResult
(
    DateTime Timestamp,
    double? Centerline,
    double? UpperEnvelope,
    double? LowerEnvelope
) : IResult;
