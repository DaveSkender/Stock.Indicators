namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class FisherTransformResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Fisher { get; set; }
    public double? Trigger { get; set; }

    double IReusableResult.Value => Fisher.Null2NaN();
}
