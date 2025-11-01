namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for Rolling Pivot Points calculations.
/// </summary>
public interface IRollingPivots
{
    /// <summary>
    /// Gets the number of periods in the rolling window.
    /// </summary>
    int WindowPeriods { get; }

    /// <summary>
    /// Gets the number of periods to offset the window.
    /// </summary>
    int OffsetPeriods { get; }

    /// <summary>
    /// Gets the type of pivot point calculation to use.
    /// </summary>
    PivotPointType PointType { get; }
}
