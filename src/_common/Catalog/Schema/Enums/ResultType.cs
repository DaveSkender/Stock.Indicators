namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the format type of result produced by a specific operation or process.
/// </summary>
/// <remarks>
/// This enumeration defines display types for indicator result types.
/// </remarks>
internal enum ResultType
{
    /// <summary>
    /// 
    /// </summary>
    Default,

    /// <summary>
    /// Used to identify the central price channel
    /// </summary>
    Centerline,

    /// <summary>
    /// Identifies one or more outlier channel boundaries
    /// </summary>
    Channel,

    /// <summary>
    /// Result should be represented as a standalone bar chart format.
    /// </summary>
    Bar,

    /// <summary>
    /// Represents a stacked bar chart visualization where data series are displayed as stacked bars.
    /// </summary>
    /// <remarks>
    /// When selected, it is expected that there are multiple results with this marker.
    /// </remarks>
    BarStacked
}
