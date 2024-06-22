namespace Skender.Stock.Indicators;

public record struct TemaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Tema { get; set; }

    readonly double IReusableResult.Value
        => Tema.Null2NaN();
}
