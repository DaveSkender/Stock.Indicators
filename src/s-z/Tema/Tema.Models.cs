namespace Skender.Stock.Indicators;

public readonly record struct TemaResult
(
    DateTime Timestamp,
    double? Tema
) : IReusable
{
    double IReusable.Value => Tema.Null2NaN();
}
