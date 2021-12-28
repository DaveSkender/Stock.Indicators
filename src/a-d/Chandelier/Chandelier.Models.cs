namespace Skender.Stock.Indicators;

[Serializable]
public class ChandelierResult : ResultBase
{
    public decimal? ChandelierExit { get; set; }
}

public enum ChandelierType
{
    Long = 0,
    Short = 1
}
