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
/// Close date/time of the aggregate
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
    [JsonIgnore]
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
    [JsonIgnore]
    public double Value => Close;
}

/// <summary>
/// Experimental Quote with internal long storage.
/// Uses decimal.ToOACurrency() for price/volume and DateTime.Ticks for timestamp.
/// Public interface remains compatible with IQuote (decimal properties).
/// For research purposes only - comparing size and performance characteristics.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="QuoteX"/> class.
/// </remarks>
/// <param name="timestamp">Close date/time of the aggregate</param>
/// <param name="open">Aggregate bar's first tick price</param>
/// <param name="high">Aggregate bar's highest tick price</param>
/// <param name="low">Aggregate bar's lowest tick price</param>
/// <param name="close">Aggregate bar's last tick price</param>
/// <param name="volume">Aggregate bar's tick volume</param>
[Serializable]
internal sealed class QuoteX(
    DateTime timestamp,
    decimal open,
    decimal high,
    decimal low,
    decimal close,
    decimal volume) : IQuote
{
    private const double OaCurrencyScale = 10000d;

    // Internal storage as long values
    private readonly long _timestampTicks = timestamp.Ticks;
    private readonly DateTimeKind _timestampKind = timestamp.Kind;

    /// <inheritdoc/>
    public DateTime Timestamp => new(_timestampTicks, _timestampKind);

    /// <inheritdoc/>
    public decimal Open => decimal.FromOACurrency(OpenLong);

    /// <inheritdoc/>
    public decimal High => decimal.FromOACurrency(HighLong);

    /// <inheritdoc/>
    public decimal Low => decimal.FromOACurrency(LowLong);

    /// <inheritdoc/>
    public decimal Close => decimal.FromOACurrency(CloseLong);

    /// <inheritdoc/>
    public decimal Volume => decimal.FromOACurrency(VolumeLong);

    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => CloseLong / OaCurrencyScale;

    /// <summary>
    /// Gets internal long representation of Open price.
    /// </summary>
    internal long OpenLong { get; } = decimal.ToOACurrency(open);

    /// <summary>
    /// Gets internal long representation of High price.
    /// </summary>
    internal long HighLong { get; } = decimal.ToOACurrency(high);

    /// <summary>
    /// Gets internal long representation of Low price.
    /// </summary>
    internal long LowLong { get; } = decimal.ToOACurrency(low);

    /// <summary>
    /// Gets internal long representation of Close price.
    /// </summary>
    internal long CloseLong { get; } = decimal.ToOACurrency(close);

    /// <summary>
    /// Gets internal long representation of Volume.
    /// </summary>
    internal long VolumeLong { get; } = decimal.ToOACurrency(volume);
}
