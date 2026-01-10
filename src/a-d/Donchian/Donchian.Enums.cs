namespace Skender.Stock.Indicators;

/// <summary>
/// Field selection for Donchian Channel indicator.
/// </summary>
public enum DonchianField
{
    /// <summary>
    /// Centerline (middle band).
    /// </summary>
    Centerline = 0,

    /// <summary>
    /// Upper band.
    /// </summary>
    UpperBand = 1,

    /// <summary>
    /// Lower band.
    /// </summary>
    LowerBand = 2,

    /// <summary>
    /// Width (difference between upper and lower bands).
    /// </summary>
    Width = 3
}
