namespace Skender.Stock.Indicators;

/// <summary>
/// Double-point precision Quote, for internal use only.
/// </summary>
/// <inheritdoc cref="Quote" />
[Serializable]
internal record QuoteD
(
    DateTime Timestamp,
    double Open,
    double High,
    double Low,
    double Close,
    double Volume
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Close;
}
