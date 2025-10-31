namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for generating Renko chart results using ATR-based brick sizing.
/// </summary>
public interface IRenkoAtr
{
    /// <summary>
    /// Gets the number of periods used for the ATR calculation.
    /// </summary>
    int AtrPeriods { get; }

    /// <summary>
    /// Gets the price candle end type used to determine when threshold
    /// is met to generate new bricks.
    /// </summary>
    EndType EndType { get; }
}
