namespace Skender.Stock.Indicators;

[Serializable]
public record VortexResult
(
    DateTime Timestamp,
    double? Pvi = null,
    double? Nvi = null
) : ISeries;
