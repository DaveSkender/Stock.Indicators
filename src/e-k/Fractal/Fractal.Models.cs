namespace Skender.Stock.Indicators;

public sealed record class FractalResult : IResult
{
    public DateTime Timestamp { get; set; }
    public decimal? FractalBear { get; set; }
    public decimal? FractalBull { get; set; }
}
