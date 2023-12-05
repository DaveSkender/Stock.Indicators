namespace Skender.Stock.Indicators;

public sealed class UltimateResult : ResultBase, IReusableResult
{
    public double? Ultimate { get; set; }

    double IReusableResult.Value => Ultimate.Null2NaN();
}
