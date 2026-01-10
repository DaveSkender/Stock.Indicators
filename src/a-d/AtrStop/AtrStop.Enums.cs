namespace Skender.Stock.Indicators;

/// <summary>
/// Field selection for ATR Trailing Stop indicator.
/// </summary>
public enum AtrStopField
{
    /// <summary>
    /// ATR Trailing Stop value.
    /// </summary>
    AtrStop = 0,

    /// <summary>
    /// Buy stop value.
    /// </summary>
    BuyStop = 1,

    /// <summary>
    /// Sell stop value.
    /// </summary>
    SellStop = 2,

    /// <summary>
    /// ATR value.
    /// </summary>
    Atr = 3
}
