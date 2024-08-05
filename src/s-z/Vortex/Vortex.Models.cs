namespace Skender.Stock.Indicators;

public record VortexResult
(
    DateTime Timestamp,
    double? Pvi = null,
    double? Nvi = null
) : ISeries;
