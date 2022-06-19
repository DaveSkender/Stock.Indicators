namespace Skender.Stock.Indicators;

[Serializable]
public sealed class FractalResult : ResultBase
{
    public decimal? FractalBear { get; set; }
    public decimal? FractalBull { get; set; }
}
