namespace Skender.Stock.Indicators;

public record TrResult(
    DateTime Timestamp,
    double? Tr
) : Reusable(Timestamp, Tr.Null2NaN());
