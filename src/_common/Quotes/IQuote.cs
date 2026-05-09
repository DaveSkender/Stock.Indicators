namespace Skender.Stock.Indicators;

/// <summary>
/// Quote interface for standard OHLCV aggregate period.
/// This is commonly known as a "bar" or "candle" and represents
/// and asset price range over a specific time range,
/// <para>
/// If implementing your own custom <c>TQuote:IQuote</c> type:
/// </para>
/// <para>
/// For chaining compatibility (<see cref="IReusable"/>
/// compliance), add the following <c>TQuote</c> property
/// (pointer) to your <see cref="Close"/> price.
/// <code>
///    double IReusable.Value => (double)Close;
/// </code>
/// </para>
/// <para>
/// TIP: If you do not need a custom quote type,
/// use the built-in <see cref="Quote"/>.
/// </para>
/// </summary>
public interface IQuote : IReusable
{
    /// <summary>
    /// Aggregate bar's first tick price
    /// </summary>
    decimal Open { get; }

    /// <summary>
    /// Aggregate bar's highest tick price
    /// </summary>
    decimal High { get; }

    /// <summary>
    /// Aggregate bar's lowest tick price
    /// </summary>
    decimal Low { get; }

    /// <summary>
    /// Aggregate bar's last tick price
    /// </summary>
    decimal Close { get; }

    /// <summary>
    /// Aggregate bar's tick volume
    /// </summary>
    decimal Volume { get; }
}
