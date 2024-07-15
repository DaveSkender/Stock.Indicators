namespace Skender.Stock.Indicators;

public record TemaResult
(
    DateTime Timestamp,
    double? Tema = null
) : Reusable(Timestamp, Tema.Null2NaN());
