namespace Skender.Stock.Indicators;

/// <summary>
/// Built-in Tick type, representing a raw market tick data point.
/// </summary>
/// <param name="Timestamp">
/// Date/time of the tick
/// </param>
/// <param name="Price">
/// Tick price
/// </param>
/// <param name="Volume">
/// Tick volume (quantity traded)
/// </param>
[Serializable]
public record Tick
(
    DateTime Timestamp,
    decimal Price,
    decimal Volume
) : ITick
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => (double)Price;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tick"/> class.
    /// </summary>
    public Tick()
        : this(default, default, default) { }
}
