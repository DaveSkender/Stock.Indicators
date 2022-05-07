namespace Skender.Stock.Indicators;

[Serializable]
public class RsiResult : ResultBase, IReusableResult
{
    public double? Rsi { get; set; }
    public double? Value
    {
        get { return Rsi; }
    }
}
