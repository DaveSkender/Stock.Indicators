namespace Skender.Stock.Indicators;

/// <summary>
/// Field selection for Pivots indicator.
/// </summary>
public enum PivotPointField
{
    /// <summary>
    /// High pivot point.
    /// </summary>
    HighPoint = 0,

    /// <summary>
    /// Low pivot point.
    /// </summary>
    LowPoint = 1,

    /// <summary>
    /// High line (connecting high points).
    /// </summary>
    HighLine = 2,

    /// <summary>
    /// Low line (connecting low points).
    /// </summary>
    LowLine = 3
}
