namespace Skender.Stock.Indicators;

public sealed class TemaResult : ResultBase, IReusableResult
{
    public double? Tema { get; set; }

    double IReusableResult.Value => Tema.Null2NaN();
}
