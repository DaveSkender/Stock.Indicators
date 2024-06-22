namespace Skender.Stock.Indicators;

public record struct ParabolicSarResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Sar { get; set; }
    public bool? IsReversal { get; set; }

    readonly double IReusableResult.Value
        => Sar.Null2NaN();
}
