namespace Skender.Stock.Indicators;

public record struct FisherTransformResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Fisher { get; set; }
    public double? Trigger { get; set; }

    readonly double IReusableResult.Value
        => Fisher.Null2NaN();
}
