namespace Skender.Stock.Indicators;

public record EmaResult
(
    DateTime Timestamp,
    double? Ema = null
) : Reusable(Timestamp, Ema.Null2NaN());
