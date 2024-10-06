namespace Skender.Stock.Indicators;

public record FcbResult
(
    DateTime Timestamp,
    decimal? UpperBand,
    decimal? LowerBand
) : ISeries;
