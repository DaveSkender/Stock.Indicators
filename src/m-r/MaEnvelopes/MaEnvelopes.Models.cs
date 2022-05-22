namespace Skender.Stock.Indicators;

public interface IMaEnvelopeResult
{
    public double? Centerline { get; }
    public double? UpperEnvelope { get; }
    public double? LowerEnvelope { get; }
}

[Serializable]
public sealed class MaEnvelopeResult : ResultBase, IMaEnvelopeResult
{
    public double? Centerline { get; set; }
    public double? UpperEnvelope { get; set; }
    public double? LowerEnvelope { get; set; }
}
