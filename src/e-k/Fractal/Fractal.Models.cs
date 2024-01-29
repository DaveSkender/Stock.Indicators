namespace Skender.Stock.Indicators;

public sealed record class FractalResult : IResult
{
    public DateTime TickDate { get; set; }
    public decimal? FractalBear { get; set; }
    public decimal? FractalBull { get; set; }
}
