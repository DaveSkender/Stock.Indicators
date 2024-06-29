namespace Skender.Stock.Indicators;

// QUOTE MODELS

/// <summary>
/// Quote interface for standard OHLCV aggregate period.
/// This is commonly known as a "bar" or "candle" and represents
/// and asset price range over a specific time range,
/// <para>
/// If implementing your own custom <c>TQuote:IQuote</c> type:
/// </para>
/// <para>
/// (A) For streaming compatibility, define it as a
/// <see langword="record struct"/> value-based type.
/// </para>
/// <para>
/// (B) For chaining compatibility (<see cref="IReusable"/>
/// compliance), add the following <c>TQuote</c> property
/// (pointer) to your <see cref="IQuote.Close"/> price.
/// <code>
///    double IReusableResult.Value => (double)Close;
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

/// <summary>
/// Built-in Quote type, representing an OHLCV aggregate price period.
/// </summary>
/// <param name="Timestamp">
/// Close date/time of the aggregate period
/// </param>
/// <param name="Open">
/// Aggregate bar's first tick price
/// </param>
/// <param name="High">
/// Aggregate bar's highest tick price
/// </param>
/// <param name="Low">
/// Aggregate bar's lowest tick price
/// </param>
/// <param name="Close">
/// Aggregate bar's last tick price
/// </param>
/// <param name="Volume">
/// Aggregate bar's tick volume
/// </param>
/// <inheritdoc cref="IQuote"/>
public readonly record struct Quote
(
    DateTime Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume
) : IQuote
{
    double IReusable.Value => (double)Close;
}

/// <summary>
/// Double-point precision Quote, for internal use only.
/// </summary>
/// <inheritdoc cref="Quote" />
internal readonly record struct QuoteD
(
    DateTime Timestamp,
    double Open,
    double High,
    double Low,
    double Close,
    double Volume
) : IReusable
{
    double IReusable.Value => Close;
}
