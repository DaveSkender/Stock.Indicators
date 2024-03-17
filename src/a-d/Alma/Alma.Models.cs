namespace Skender.Stock.Indicators;

public sealed record class AlmaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Alma { get; set; }

    double IReusableResult.Value => Alma.Null2NaN();
}
