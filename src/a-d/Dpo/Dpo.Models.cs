namespace Skender.Stock.Indicators;

public record DpoResult
(
    DateTime Timestamp,
    double? Dpo = null,
    double? Sma = null
    ) : Reusable(Timestamp, Dpo.Null2NaN());
