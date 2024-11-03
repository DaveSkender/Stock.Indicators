namespace Skender.Stock.Indicators;

[Serializable]
public record UltimateResult
(
    DateTime Timestamp,
    double? Ultimate
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Ultimate.Null2NaN();
}
