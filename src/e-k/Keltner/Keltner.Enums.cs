namespace Skender.Stock.Indicators;

/// <summary>
/// Field selection for Keltner Channel indicator.
/// </summary>
public enum KeltnerField
{
    /// <summary>
    /// Upper band.
    /// </summary>
    UpperBand = 0,

    /// <summary>
    /// Centerline (moving average).
    /// </summary>
    Centerline = 1,

    /// <summary>
    /// Lower band.
    /// </summary>
    LowerBand = 2,

    /// <summary>
    /// Width (difference between upper and lower bands).
    /// </summary>
    Width = 3
}
