namespace Skender.Stock.Indicators;

public record ObvResult
(
    DateTime Timestamp,
    double Obv
) : Reusable(Timestamp, Obv);
