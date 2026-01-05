namespace Skender.Stock.Indicators;

/// <summary>
/// Field selection for Alligator indicator.
/// </summary>
public enum AlligatorLine
{
    /// <summary>
    /// Jaw line (blue line, 13-period SMMA shifted 8 periods forward).
    /// </summary>
    Jaw = 0,

    /// <summary>
    /// Teeth line (red line, 8-period SMMA shifted 5 periods forward).
    /// </summary>
    Teeth = 1,

    /// <summary>
    /// Lips line (green line, 5-period SMMA shifted 3 periods forward).
    /// </summary>
    Lips = 2
}
