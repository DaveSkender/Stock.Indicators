namespace Skender.Stock.Indicators;

public record SmiResult
(
    DateTime Timestamp,
    double? Smi,
    double? Signal
) : Reusable(Timestamp, Smi.Null2NaN());
