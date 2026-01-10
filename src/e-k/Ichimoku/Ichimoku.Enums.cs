namespace Skender.Stock.Indicators;

/// <summary>
/// Line selection for Ichimoku Cloud indicator.
/// </summary>
public enum IchimokuLine
{
    /// <summary>
    /// Tenkan-sen (Conversion Line) - 9-period midpoint.
    /// </summary>
    TenkanSen = 0,

    /// <summary>
    /// Kijun-sen (Base Line) - 26-period midpoint.
    /// </summary>
    KijunSen = 1,

    /// <summary>
    /// Senkou Span A (Leading Span A) - average of Tenkan and Kijun, projected forward.
    /// </summary>
    SenkouSpanA = 2,

    /// <summary>
    /// Senkou Span B (Leading Span B) - 52-period midpoint, projected forward.
    /// </summary>
    SenkouSpanB = 3,

    /// <summary>
    /// Chikou Span (Lagging Span) - Close price projected backwards.
    /// </summary>
    ChikouSpan = 4
}
