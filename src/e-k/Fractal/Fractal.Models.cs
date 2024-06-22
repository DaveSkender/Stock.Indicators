namespace Skender.Stock.Indicators;

public record struct FractalResult : IResult
{
    public DateTime Timestamp { get; set; }
    public decimal? FractalBear { get; set; }
    public decimal? FractalBull { get; set; }
}
