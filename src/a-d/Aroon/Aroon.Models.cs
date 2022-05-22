namespace Skender.Stock.Indicators;

public interface IAroonResult
{
    public double? AroonUp { get; set; }
    public double? AroonDown { get; set; }
    public double? Oscillator { get; set; }
}

[Serializable]
public sealed class AroonResult : ResultBase, IAroonResult, IReusableResult
{
    public double? AroonUp { get; set; }
    public double? AroonDown { get; set; }
    public double? Oscillator { get; set; }

    double? IReusableResult.Value => Oscillator;
}
