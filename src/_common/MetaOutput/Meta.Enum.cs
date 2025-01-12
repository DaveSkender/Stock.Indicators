/// <summary>
/// Enum for the type of chart.
/// </summary>
[Serializable]
public enum ChartType
{
    /// <summary>
    /// Results are displayed as an overlay on the price chart.
    /// </summary>
    Overlay,
    /// <summary>
    /// Results are displayed in a separate indicator panel.
    /// </summary>
    Oscillator
}

/// <summary>
/// Enum for the kind of indicator methods.
/// </summary>
[Serializable]
public enum Kind
{
    /// <summary>
    /// Series indicator kind.
    /// </summary>
    Series,

    /// <summary>
    /// Buffer indicator kind.
    /// </summary>
    Buffer,

    /// <summary>
    /// Stream indicator kind.
    /// </summary>
    Stream
}

/// <summary>
/// Enum for the category of the indicator.
/// </summary>
[Serializable]
public enum Category
{
    /// <summary>
    /// Patterns used in candlestick charting to predict price movement.
    /// </summary>
    CandlestickPattern,

    /// <summary>
    /// A statistical calculation to analyze data points by creating a series of averages.
    /// </summary>
    MovingAverage,

    /// <summary>
    /// A technical analysis tool that varies over time within a band.
    /// </summary>
    Oscillator,

    /// <summary>
    /// Indicators that use the same scale as prices and are plotted over the top of the price bars.
    /// </summary>
    Overlay,

    /// <summary>
    /// A range within which a security's price tends to stay for a period of time.
    /// </summary>
    PriceChannel,

    /// <summary>
    /// Multiple price channels used for technical analysis.
    /// </summary>
    PriceChannels,

    /// <summary>
    /// Attributes or features of a security's price.
    /// </summary>
    PriceCharacteristic,

    /// <summary>
    /// Patterns formed by the price movements of a security.
    /// </summary>
    PricePattern,

    /// <summary>
    /// Transformations applied to price data for analysis.
    /// </summary>
    PriceTransform,

    /// <summary>
    /// The general direction in which a security's price is moving.
    /// </summary>
    PriceTrend,

    /// <summary>
    /// Indicators used to determine stop and reverse points in trading.
    /// </summary>
    StopAndReverse,

    /// <summary>
    /// Indicators that use volume data to analyze price movements.
    /// </summary>
    VolumeBased
}

[Serializable]
public enum Order
{
    // price is 75/76
    // thresholds are 99
    Front = 1,
    Behind = 50,
    BehindPrice = 80,
    Back = 95
}
