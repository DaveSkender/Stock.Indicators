namespace Skender.Stock.Indicators;

// QUOTE MODELS

/// <summary>
/// Quote interface for standard OHLCV aggregate period.
/// <para>
/// If implementing your own custom <c>TQuote:IQuote</c> type:
/// </para>
/// <para>
/// (A) We recommend defining it as a <see langword="record struct"/>
/// to ensure chaining and streaming compatibility.
/// To support chaining features, it has to have value-based
/// equality <c><see cref="IEquatable{IQuote}"/></c> and implement
/// the <c><see cref="IReusableResult"/>.Value</c> pointer to your
/// <see cref="IQuote.Close"/> price.  For streaming, it also has
/// to be a <see langword="struct"/> type.
/// </para>
/// <para>
/// (B) For <see cref="IReusableResult"/> compliance,
/// add the following <c>TQuote</c> property (pointer) to your
/// <see cref="IQuote.Close"/> price.
/// <code>
///    double IReusableResult.Value => (double)Close;
/// </code>
/// </para>
/// <para>
/// TIP: If you do not need customization,
/// use our built-in <see cref="Quote"/> type.
/// </para>
/// </summary>
public interface IQuote : IEquatable<IQuote>, IReusableResult
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

    // reminder: IEquatable does not enforce use of == and != operators,
    // so any internal comparisons should always use the Equals() method.
    // Use of those operators would be reference-based equality only if
    // users opt for 'class' type and did not define them.
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
public record struct Quote(
    DateTime Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume)
    : IQuote, IReusableResult
{
    readonly double IReusableResult.Value
        => (double)Close;

    // this is only an appropriate
    // implementation for record types
    public readonly bool Equals(IQuote? other)
      => base.Equals(other);
}

/// <summary>
/// Double-point precision Quote, for internal use only.
/// </summary>
/// <inheritdoc cref="Quote" />
internal record struct QuoteD(
    DateTime Timestamp,
    double Open,
    double High,
    double Low,
    double Close,
    double Volume) : IReusableResult
{
    readonly double IReusableResult.Value
        => Close;
}
