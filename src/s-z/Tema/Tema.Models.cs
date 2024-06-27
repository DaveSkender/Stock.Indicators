namespace Skender.Stock.Indicators;

public record struct TemaResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Tema { get; set; }

    readonly double IReusable.Value
        => Tema.Null2NaN();
}
