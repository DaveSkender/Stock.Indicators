namespace Skender.Stock.Indicators;

internal enum Style
{
    Series,
    Buffer,
    Stream
}

/// <summary>
/// Represents the category of an indicator.
/// </summary>
public enum Category
{
    /// <summary>
    /// Undefined category.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Indicators related to candlestick patterns.
    /// </summary>
    CandlestickPattern,

    /// <summary>
    /// Indicators related to moving averages.
    /// </summary>
    MovingAverage,

    /// <summary>
    /// Indicators that are oscillators.
    /// </summary>
    Oscillator,

    /// <summary>
    /// Indicators related to price channels.
    /// </summary>
    PriceChannel,

    /// <summary>
    /// Indicators that describe price characteristics.
    /// </summary>
    PriceCharacteristic,

    /// <summary>
    /// Indicators related to price patterns.
    /// </summary>
    PricePattern,

    /// <summary>
    /// Indicators that transform price data.
    /// </summary>
    PriceTransform,

    /// <summary>
    /// Indicators that analyze price trends.
    /// </summary>
    PriceTrend,

    /// <summary>
    /// Indicators related to stop and reverse strategies.
    /// </summary>
    StopAndReverse,

    /// <summary>
    /// Indicators based on volume data.
    /// </summary>
    VolumeBased,
}

/// <summary>
/// Represents the type of chart used for an indicator.
/// </summary>
public enum ChartType
{
    /// <summary>
    /// Undefined chart type.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Overlay chart type.
    /// </summary>
    Overlay,

    /// <summary>
    /// Oscillator chart type.
    /// </summary>
    Oscillator
}
