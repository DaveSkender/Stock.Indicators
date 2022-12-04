namespace Skender.Stock.Indicators;

[Serializable]
public sealed class T3Result : ResultBase, IReusableResult
{
    public T3Result(DateTime date)
    {
        Date = date;
    }

    public double? T3 { get; set; }

    double? IReusableResult.ChainValue => T3;
}
