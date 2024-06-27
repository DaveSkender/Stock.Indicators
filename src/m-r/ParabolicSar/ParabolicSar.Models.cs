namespace Skender.Stock.Indicators;

public record struct ParabolicSarResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Sar { get; set; }
    public bool? IsReversal { get; set; }

    readonly double IReusable.Value
        => Sar.Null2NaN();
}
