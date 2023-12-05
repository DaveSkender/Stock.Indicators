namespace Skender.Stock.Indicators;

public sealed class UltimateResult : ResultBase, IReusableResult
{
    public UltimateResult(DateTime date)
    {
        Date = date;
    }

    public double? Ultimate { get; set; }

    double IReusableResult.Value => Ultimate.Null2NaN();
}
