namespace Skender.Stock.Indicators;

public sealed record class FisherTransformResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Fisher { get; set; }
    public double? Trigger { get; set; }

    double IReusableResult.Value => Fisher.Null2NaN();
}
