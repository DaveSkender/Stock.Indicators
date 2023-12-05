namespace Skender.Stock.Indicators;

public sealed class FisherTransformResult : ResultBase, IReusableResult
{
    public double? Fisher { get; set; }
    public double? Trigger { get; set; }

    double IReusableResult.Value => Fisher.Null2NaN();
}
