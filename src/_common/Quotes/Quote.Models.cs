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
[Serializable]
public record Quote
(
    DateTime Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume
) : IQuote
{
    /// <inheritdoc/>
    public double Value => (double)Close;

    /// <inheritdoc/>
    [Obsolete("Use 'Timestamp' property instead.")]
    public DateTime Date
    {
        get => Timestamp;
        init => Timestamp = value;
    }

    // TODO: this can be removed when Date is removed
    // It allows old `new Quote { Date: ... }` initialization.
    /// <inheritdoc/>
    public Quote()
        : this(default, default, default, default, default, default) { }
}

/// <summary>
/// Double-point precision Quote, for internal use only.
/// </summary>
/// <inheritdoc cref="Quote" />
[Serializable]
internal record QuoteD
(
    DateTime Timestamp,
    double Open,
    double High,
    double Low,
    double Close,
    double Volume
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Close;
}
