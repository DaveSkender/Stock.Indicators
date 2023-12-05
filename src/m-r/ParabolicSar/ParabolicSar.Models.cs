namespace Skender.Stock.Indicators;

public sealed class ParabolicSarResult : ResultBase, IReusableResult
{
    public ParabolicSarResult(DateTime date)
    {
        Date = date;
    }

    public double? Sar { get; set; }
    public bool? IsReversal { get; set; }

    double IReusableResult.Value => Sar.Null2NaN();
}
