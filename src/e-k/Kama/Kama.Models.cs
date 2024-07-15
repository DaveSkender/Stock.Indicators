namespace Skender.Stock.Indicators;

public record KamaResult
(
    DateTime Timestamp,
    double? Er = null,
    double? Kama = null
) : Reusable(Timestamp, Kama.Null2NaN());
