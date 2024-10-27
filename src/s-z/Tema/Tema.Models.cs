namespace Skender.Stock.Indicators;

public record TemaResult
(
    DateTime Timestamp,
    double? Tema = null
) : IReusable
{
    public double Value => Tema.Null2NaN();
}
