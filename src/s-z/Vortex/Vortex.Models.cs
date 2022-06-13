namespace Skender.Stock.Indicators;

[Serializable]
public sealed class VortexResult : ResultBase
{
    public double? Pvi { get; set; }
    public double? Nvi { get; set; }
}
