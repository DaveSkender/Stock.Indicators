namespace Skender.Stock.Indicators;

[Serializable]
public record FcbResult
(
    DateTime Timestamp,
    decimal? UpperBand,
    decimal? LowerBand
) : ISeries;
