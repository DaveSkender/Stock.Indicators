namespace Skender.Stock.Indicators;

public record EpmaResult
(
    DateTime Timestamp,
    double? Epma
) : Reusable(Timestamp, Epma.Null2NaN());
