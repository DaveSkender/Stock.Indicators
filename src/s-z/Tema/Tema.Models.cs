namespace Skender.Stock.Indicators;

[Serializable]
public record TemaResult
(
    DateTime Timestamp,
    double? Tema = null
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Tema.Null2NaN();
}
