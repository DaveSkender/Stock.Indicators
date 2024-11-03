namespace Skender.Stock.Indicators;

[Serializable]
public record DpoResult
(
    DateTime Timestamp,
    double? Dpo = null,
    double? Sma = null
    ) : IReusable
{
    /// <inheritdoc/>
    public double Value => Dpo.Null2NaN();
}
