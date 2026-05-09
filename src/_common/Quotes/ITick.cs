namespace Skender.Stock.Indicators;

/// <summary>
/// Tick interface for raw market tick data.
/// This represents a single trade or quote event with price and volume at a specific timestamp.
/// </summary>
public interface ITick : IReusable
{
    /// <summary>
    /// Tick price
    /// </summary>
    decimal Price { get; }

    /// <summary>
    /// Tick volume (quantity traded)
    /// </summary>
    decimal Volume { get; }

    /// <summary>
    /// Optional unique execution ID for duplicate detection.
    /// When null, duplicates are assessed by timestamp only.
    /// </summary>
    string? ExecutionId { get; }
}
