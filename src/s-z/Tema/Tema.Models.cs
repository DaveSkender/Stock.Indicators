namespace Skender.Stock.Indicators;

public sealed record class TemaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Tema { get; set; }

    double IReusableResult.Value => Tema.Null2NaN();
}
