namespace Skender.Stock.Indicators;

public record TemaResult
(
    DateTime Timestamp,
    double? Tema = null
) : Reusable(Timestamp)
{
    public override double Value => Tema.Null2NaN();
}
