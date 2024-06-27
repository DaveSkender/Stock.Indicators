namespace Skender.Stock.Indicators;

public record struct FisherTransformResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Fisher { get; set; }
    public double? Trigger { get; set; }

    readonly double IReusable.Value
        => Fisher.Null2NaN();
}
