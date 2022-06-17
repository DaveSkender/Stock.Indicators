namespace Skender.Stock.Indicators;

[Serializable]
public class KamaResult : MaResultBase
{
    public double? ER { get; set; }
    public decimal? Kama { get; set; }
}
