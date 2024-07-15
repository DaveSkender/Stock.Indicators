namespace Skender.Stock.Indicators;

public record MamaResult
(
    DateTime Timestamp,
    double? Mama = null,
    double? Fama = null
) : Reusable(Timestamp, Mama.Null2NaN());
