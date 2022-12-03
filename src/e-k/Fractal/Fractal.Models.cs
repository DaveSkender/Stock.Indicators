namespace Skender.Stock.Indicators;

[Serializable]
public sealed class FractalResult : ResultBase
{
    public FractalResult(DateTime date)
    {
        Date = date;
    }

    public decimal? FractalBear { get; set; }
    public decimal? FractalBull { get; set; }
}
