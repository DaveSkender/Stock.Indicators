namespace Skender.Stock.Indicators;

public sealed class MfiResult : ResultBase, IReusableResult
{
    public double? Mfi { get; set; }

    double IReusableResult.Value => Mfi.Null2NaN();
}
