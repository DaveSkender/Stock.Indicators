namespace Skender.Stock.Indicators;

[Serializable]
public sealed class MaEnvelopeResult : ResultBase
{
    public MaEnvelopeResult(DateTime date)
    {
        Date = date;
    }

    public double? Centerline { get; set; }
    public double? UpperEnvelope { get; set; }
    public double? LowerEnvelope { get; set; }
}
