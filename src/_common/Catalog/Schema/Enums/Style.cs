namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the style of an indicator.
/// </summary>
/// <remarks>
/// Values may be hardcoded in the catalog
/// generator and should not be changed.
/// </remarks>
public enum Style
{
    /// <summary>
    /// Represents a series chart type in a charting library.
    /// </summary>
    Series = 0,

    /// <summary>
    /// Represents a buffer mode for data processing or storage.
    /// </summary>
    Buffer = 1,

    /// <summary>
    /// Represents a mode where data is streamed continuously.
    /// </summary>
    Stream = 2
}
