namespace Skender.Stock.Indicators;

/// <summary>
/// TDIGM result containing all indicator series.
/// Value points to the fast MA (most relevant for trading signals).
/// Implements IReusable to enable chaining in Skender v3.
/// </summary>
[Serializable]
public record TdiGmResult() : IReusable
{
    public required DateTime Timestamp { get; init; }
    public double? Upper { get; init; }
    public double? Lower { get; init; }
    public double? Middle { get; init; }
    public double? Slow { get; init; }
    public double? Fast { get; init; }

    [JsonIgnore]
    public double Value => Fast.Null2NaN();
}
