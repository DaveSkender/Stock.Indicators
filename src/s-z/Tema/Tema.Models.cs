namespace Skender.Stock.Indicators;

[Serializable]
public sealed class TemaResult : ResultBase, IReusableResult
{
    public TemaResult(DateTime date)
    {
        Date = date;
    }

    public double? Tema { get; set; }

    double? IReusableResult.Value => Tema;
}
